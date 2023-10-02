using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;

namespace NonPersistentObjectsDemo.Module.BusinessObjects {

    [DomainComponent]
    [DefaultProperty(nameof(Group.Name))]
    public class Group {
        [DevExpress.ExpressApp.Data.Key]
        [Browsable(true)]
        public string Name { get; set; }
    }

    class NPGroupAdapter {
        private NonPersistentObjectSpace _objectSpace;
        protected NonPersistentObjectSpace ObjectSpace { get { return _objectSpace; } }
        private List<Group> objects;
        public NPGroupAdapter(NonPersistentObjectSpace npos) {
            this._objectSpace = npos;
            _objectSpace.ObjectsGetting += ObjectSpace_ObjectsGetting;
            _objectSpace.ObjectByKeyGetting += ObjectSpace_ObjectByKeyGetting;
        }
        protected Group GetObjectByKey(string key) {
            return new Group() { Name = key };
        }
        private void ObjectSpace_ObjectByKeyGetting(object sender, ObjectByKeyGettingEventArgs e) {
            if(e.Key != null) {
                if(e.ObjectType == typeof(Group)) {
                    e.Object = GetObjectByKey((string)e.Key);
                }
            }
        }
        private void ObjectSpace_ObjectsGetting(object sender, ObjectsGettingEventArgs e) {
            if(e.ObjectType == typeof(Group)) {
                if(objects == null) {
                    var pos = ObjectSpace.Owner as IObjectSpace;
                    objects = pos.GetObjectsQuery<Product>().Where(o => o.GroupName != null).GroupBy(o => o.GroupName).Select(o => GetObjectByKey(o.Key)).ToList();
                }
                e.Objects = objects;
            }
        }
    }
}
