using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.EF;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace NonPersistentObjectsDemo.Module.BusinessObjects {

    [DevExpress.ExpressApp.DC.XafDefaultProperty(nameof(Project.CodeName))]
    [DefaultClassOptions]
    public class Project : BaseObject {
        public Project() {
            ((INotifyPropertyChanged)this).PropertyChanged += Project_PropertyChanged;
        }



        public virtual string CodeName {            get; set;        }

        #region Features
        private BindingList<Feature> _Features;
        [NotMapped]

        public virtual BindingList<Feature> Features {
            get {
                if(_Features == null) {
                    _Features = new BindingList<Feature>();
                    _Features.ListChanged += _Features_ListChanged;
                }
                return _Features;
            }
        }
        private void _Features_ListChanged(object sender, ListChangedEventArgs e) {
            var list = (BindingList<Feature>)sender;
            if(e.ListChangedType == ListChangedType.ItemAdded) {
                var obj = list[e.NewIndex];
                obj.OwnerKey = this.ID;
                obj.LocalKey = e.NewIndex + 1;
            }
            FeatureList = SerializationHelper.Save(Features);
        }
        [Browsable(false)]
        public virtual string FeatureList { get; set; }
        #endregion

        #region MainFeature
        [DataSourceProperty(nameof(Features))]
        [NotMapped]
        public virtual Feature MainFeature { get; set; }
        [Browsable(false)]
        public virtual string MainFeatureName { get; set; }
        #endregion

        #region Resources
        private BindingList<Resource> _Resources;
        [NotMapped]
        public virtual BindingList<Resource> Resources {
            get {
                if(_Resources == null) {
                    _Resources = new BindingList<Resource>();
                    _Resources.ListChanged += _Resources_ListChanged;
                }
                return _Resources;
            }
        }
        private void _Resources_ListChanged(object sender, ListChangedEventArgs e) {
            var list = (BindingList<Resource>)sender;
            if(e.ListChangedType == ListChangedType.ItemAdded) {
                list[e.NewIndex].OwnerKey = this.ID;
            }
            ResourceList = SerializationHelper.Save(Resources);
        }
        [Browsable(false)]
        public virtual string ResourceList { get; set; }
        #endregion



        private void Project_PropertyChanged(object sender, PropertyChangedEventArgs e) {

            if(e.PropertyName == nameof(MainFeature)) {
                MainFeatureName = this.MainFeature?.Name;
            }
        }
        public override void OnLoaded() {
            base.OnLoaded();
            int counter = 0;
            SerializationHelper.Load(Features, FeatureList, ObjectSpace, o => { o.OwnerKey = this.ID; o.LocalKey = ++counter; });
            SerializationHelper.Load(Resources, ResourceList, ObjectSpace, o => { o.OwnerKey = this.ID; });
            MainFeature = MainFeatureName == null ? null : Features.FirstOrDefault(f => f.Name == MainFeatureName);
        }

        public override void OnSaving() {
            FeatureList = SerializationHelper.Save(Features);
            ResourceList = SerializationHelper.Save(Resources);
            base.OnSaving();
        }

    }
}
