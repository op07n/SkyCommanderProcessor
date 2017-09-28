using System;
using System.Reflection;
using System.Reflection.Emit;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml.Serialization;

namespace SkyCommanderProcessor.EXLogic {

  public static class Cloner<T> {
    private static Func<T, T> cloner = CreateCloner();

    private static Func<T, T> CreateCloner() {
      var cloneMethod = new DynamicMethod("CloneImplementation", typeof(T), new Type[] { typeof(T) }, true);
      var defaultCtor = typeof(T).GetConstructor(new Type[] { });

      var generator = cloneMethod.GetILGenerator();

      var loc1 = generator.DeclareLocal(typeof(T));

      generator.Emit(OpCodes.Newobj, defaultCtor);
      generator.Emit(OpCodes.Stloc, loc1);

      foreach (var field in typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
        generator.Emit(OpCodes.Ldloc, loc1);
        generator.Emit(OpCodes.Ldarg_0);
        generator.Emit(OpCodes.Ldfld, field);
        generator.Emit(OpCodes.Stfld, field);
      }

      generator.Emit(OpCodes.Ldloc, loc1);
      generator.Emit(OpCodes.Ret);

      return ((Func<T, T>)cloneMethod.CreateDelegate(typeof(Func<T, T>)));
    }

    public static T Clone(T myObject) {
      return cloner(myObject);
    }
  }

  /*
  public interface IClone {
    T Clone<T>(T instance) where T : class;
  }

  public class CloneManager : IClone {
    /// <summary>
    /// Clones the specified instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="instance">The instance.</param>
    /// <returns>A new instance of an object.</returns>
    T IClone.Clone<T>(T instance) {
      XmlSerializer serializer = new XmlSerializer(typeof(T));
      MemoryStream stream = new MemoryStream();
      serializer.Serialize(stream, instance);
      stream.Seek(0, SeekOrigin.Begin);
      return serializer.Deserialize(stream) as T;
    }
  }

  */
}
