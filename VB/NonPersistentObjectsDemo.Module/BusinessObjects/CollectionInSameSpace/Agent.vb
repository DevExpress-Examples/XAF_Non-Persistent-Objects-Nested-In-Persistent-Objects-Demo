Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Xml.Serialization
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.Persistent.Base

Namespace NonPersistentObjectsDemo.Module.BusinessObjects

	<DomainComponent>
	Public Class Agent
		Inherits NonPersistentObjectImpl

		Public Shared Sequence As Integer
		Public Sub New()
			MyBase.New()
		End Sub
		<VisibleInDetailView(False)>
		<VisibleInListView(False)>
		<XmlIgnore>
		<DevExpress.ExpressApp.Data.Key>
		Public Property ID() As Integer
		Private _Name As String
		Public Property Name() As String
			Get
				Return _Name
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)(_Name, value)
			End Set
		End Property
		Private _Progress As Double
		Public Property Progress() As Double
			Get
				Return _Progress
			End Get
			Set(ByVal value As Double)
				SetPropertyValue(Of Double)(_Progress, value)
			End Set
		End Property
	End Class

	Friend Class NPAgentAdapter
		Inherits NonPersistentObjectAdapter(Of Agent, Integer)

		Public Sub New(ByVal npos As NonPersistentObjectSpace)
			MyBase.New(npos)
		End Sub
		Protected Overrides Function LoadSameObject(ByVal obj As Agent) As Agent
			Dim result As New Agent()
			result.ID = obj.ID
			result.Name = obj.Name
			result.Progress = obj.Progress
			Return result
		End Function
		Protected Overrides ReadOnly Property ThrowOnAcceptingMismatchedObject() As Boolean
			Get
				Return True
			End Get
		End Property
	End Class

'    
'     * Also, see the overridden GetObjectSpaceToShowDetailViewFrom method
'     * in the XafApplication (WinApplication / WebApplication) descendant.
'     * 
'     
End Namespace
