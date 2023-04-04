using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DevExpress.ExpressApp;

namespace NonPersistentObjectsDemo.Module.BusinessObjects {

    static class SerializationHelper {
        public static void Load<T>(BindingList<T> list, string data, IObjectSpace objectSpace, Action<T> acceptor) {
            list.RaiseListChangedEvents = false;
            list.Clear();
            if(data != null) {
                var serializer = new XmlSerializer(typeof(T).MakeArrayType());
                using(var stream = new MemoryStream(Encoding.UTF8.GetBytes(data))) {
                    var objs = serializer.Deserialize(stream) as IList<T>;
                    foreach(var obj in objs) {
                        acceptor?.Invoke(obj);
                        var tobj = objectSpace.GetObject(obj);
                        var aobj = tobj as IAssignable<T>;
                        if(aobj != null) {
                            aobj.Assign(obj);
                        }
                        list.Add(tobj);
                    }
                }
            }
            list.RaiseListChangedEvents = true;
            list.ResetBindings();
        }
        public static string Save<T>(IList<T> list) {
            if(list == null || list.Count == 0) {
                return null;
            }
            var serializer = new XmlSerializer(typeof(T).MakeArrayType());
            using(var stream = new MemoryStream()) {
                serializer.Serialize(stream, list.ToArray());
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
