﻿using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Xpo;
using NonPersistentDemo.Blazor.Server.Services;

namespace NonPersistentDemo.Blazor.Server;

public class NonPersistentDemoBlazorApplication : BlazorApplication {
    public NonPersistentDemoBlazorApplication() {
        ApplicationName = "NonPersistentDemo";
        CheckCompatibilityType = DevExpress.ExpressApp.CheckCompatibilityType.DatabaseSchema;
        DatabaseVersionMismatch += NonPersistentDemoBlazorApplication_DatabaseVersionMismatch;
    }
    protected override void OnSetupStarted() {
        base.OnSetupStarted();
#if DEBUG
        if(System.Diagnostics.Debugger.IsAttached && CheckCompatibilityType == CheckCompatibilityType.DatabaseSchema) {
            DatabaseUpdateMode = DatabaseUpdateMode.UpdateDatabaseAlways;
        }
#endif
    }
    public override IObjectSpace GetObjectSpaceToShowDetailViewFrom(Frame sourceFrame, Type objectType, TargetWindow targetWindow) {
        if(sourceFrame.View is ListView &&
            sourceFrame.View.ObjectTypeInfo.Type == typeof(NonPersistentObjectsDemo.Module.BusinessObjects.Agent) &&
            objectType == typeof(NonPersistentObjectsDemo.Module.BusinessObjects.Agent)) {
            return sourceFrame.View.ObjectSpace;
        }
        return base.GetObjectSpaceToShowDetailViewFrom(sourceFrame, objectType, targetWindow);
    }
    private void NonPersistentDemoBlazorApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
#if EASYTEST
        e.Updater.Update();
        e.Handled = true;
#else
        if(System.Diagnostics.Debugger.IsAttached) {
            e.Updater.Update();
            e.Handled = true;
        }
        else {
            string message = "The application cannot connect to the specified database, " +
                "because the database doesn't exist, its version is older " +
                "than that of the application or its schema does not match " +
                "the ORM data model structure. To avoid this error, use one " +
                "of the solutions from the https://www.devexpress.com/kb=T367835 KB Article.";

            if(e.CompatibilityError != null && e.CompatibilityError.Exception != null) {
                message += "\r\n\r\nInner exception: " + e.CompatibilityError.Exception.Message;
            }
            throw new InvalidOperationException(message);
        }
#endif
    }
}
