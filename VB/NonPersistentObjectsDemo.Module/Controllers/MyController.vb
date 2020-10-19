Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.Actions
Imports DevExpress.ExpressApp.SystemModule
Imports NonPersistentObjectsDemo.Module.BusinessObjects

Namespace NonPersistentObjectsDemo.Module.Controllers
	Public Class FeatureListViewController
		Inherits ObjectViewController(Of ListView, Feature)

		Protected Overrides Sub OnActivated()
			MyBase.OnActivated()
			Dim filterController = Frame.GetController(Of FilterController)()
			If filterController IsNot Nothing Then
				filterController.AllowFilterNonPersistentObjects = True
			End If
		End Sub
	End Class
End Namespace
