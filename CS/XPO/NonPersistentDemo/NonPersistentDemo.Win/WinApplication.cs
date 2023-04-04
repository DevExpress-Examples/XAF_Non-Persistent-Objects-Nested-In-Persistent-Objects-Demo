using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ApplicationBuilder;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Win.Utils;
using DevExpress.ExpressApp.Xpo;

namespace NonPersistentDemo.Win;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Win.WinApplication._members
public class NonPersistentDemoWindowsFormsApplication : WinApplication {
    public NonPersistentDemoWindowsFormsApplication() {
		SplashScreen = new DXSplashScreen(typeof(XafSplashScreen), new DefaultOverlayFormOptions());
        ApplicationName = "NonPersistentDemo";
        CheckCompatibilityType = DevExpress.ExpressApp.CheckCompatibilityType.DatabaseSchema;
        UseOldTemplates = false;
        DatabaseVersionMismatch += NonPersistentDemoWindowsFormsApplication_DatabaseVersionMismatch;
        CustomizeLanguagesList += NonPersistentDemoWindowsFormsApplication_CustomizeLanguagesList;
    }
    private void NonPersistentDemoWindowsFormsApplication_CustomizeLanguagesList(object sender, CustomizeLanguagesListEventArgs e) {
        string userLanguageName = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
        if(userLanguageName != "en-US" && e.Languages.IndexOf(userLanguageName) == -1) {
            e.Languages.Add(userLanguageName);
        }
    }

    public override IObjectSpace GetObjectSpaceToShowDetailViewFrom(Frame sourceFrame, Type objectType, TargetWindow targetWindow) {
        if(sourceFrame.View is ListView &&
            sourceFrame.View.ObjectTypeInfo.Type == typeof(NonPersistentObjectsDemo.Module.BusinessObjects.Agent) &&
            objectType == typeof(NonPersistentObjectsDemo.Module.BusinessObjects.Agent)) {
            return sourceFrame.View.ObjectSpace;
        }
        return base.GetObjectSpaceToShowDetailViewFrom(sourceFrame, objectType, targetWindow);
    }
    private void NonPersistentDemoWindowsFormsApplication_DatabaseVersionMismatch(object sender, DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs e) {
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
				"because the database doesn't exist, its version is older " +
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
