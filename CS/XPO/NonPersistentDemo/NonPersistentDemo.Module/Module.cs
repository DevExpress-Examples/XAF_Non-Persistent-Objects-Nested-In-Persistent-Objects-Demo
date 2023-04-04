using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using NonPersistentObjectsDemo.Module.BusinessObjects;

namespace NonPersistentDemo.Module;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ModuleBase.
public sealed class NonPersistentDemoModule : ModuleBase {
    public NonPersistentDemoModule() {
		// 
		// NonPersistentDemoModule
		// 
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.SystemModule.SystemModule));
		RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule));
    }
    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
        ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
        return new ModuleUpdater[] { updater };
    }
    public override void Setup(XafApplication application) {
        base.Setup(application);
        application.SetupComplete += Application_SetupComplete;
        // Manage various aspects of the application UI and behavior at the module level.
    }
    private void Application_SetupComplete(object sender, EventArgs e) {
        Application.ObjectSpaceCreated += Application_ObjectSpaceCreated;
        NonPersistentObjectSpace.UseKeyComparisonToDetermineIdentity = true;
    }
    private void Application_ObjectSpaceCreated(object sender, ObjectSpaceCreatedEventArgs e) {
        var cos = e.ObjectSpace as CompositeObjectSpace;
        if(cos != null) {
            if(!(cos.Owner is CompositeObjectSpace)) {
                cos.PopulateAdditionalObjectSpaces((XafApplication)sender);
                cos.AutoCommitAdditionalObjectSpaces = true;
                cos.AutoRefreshAdditionalObjectSpaces = true;
            }
        }
        var npos = e.ObjectSpace as NonPersistentObjectSpace;
        if(npos != null) {
            npos.AutoSetModifiedOnObjectChange = true;
            new NPGroupAdapter(npos);
            new NPFeatureAdapter(npos);
            new NPResourceAdapter(npos);
            new NPAgentAdapter(npos);
            new NPTechnologyAdapter(npos);
        }
    }
    public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
        base.CustomizeTypesInfo(typesInfo);
        CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
    }
}
