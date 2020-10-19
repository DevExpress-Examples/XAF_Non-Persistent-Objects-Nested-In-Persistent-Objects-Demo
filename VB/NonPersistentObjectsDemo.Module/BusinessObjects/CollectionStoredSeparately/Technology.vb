Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.Persistent.Base

Namespace NonPersistentObjectsDemo.Module.BusinessObjects

	<DomainComponent>
	Public Class Technology
		Inherits NonPersistentBaseObject
		Implements IAssignable(Of Technology)

		Public Sub New()
			MyBase.New()
		End Sub
		Public Sub New(ByVal oid As Guid)
			MyBase.New(oid)
		End Sub
		Private _Name As String
		Public Property Name() As String
			Get
				Return _Name
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)(_Name, value)
			End Set
		End Property
		Private _Description As String
		<FieldSize(FieldSizeAttribute.Unlimited)>
		Public Property Description() As String
			Get
				Return _Description
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)(_Description, value)
			End Set
		End Property
		Public Sub Assign(ByVal source As Technology) Implements IAssignable(Of Technology).Assign
			Name = source.Name
			Description = source.Description
		End Sub
	End Class

	Friend Class NPTechnologyAdapter
		Inherits NonPersistentObjectAdapter(Of Technology, Guid)

		Private Shared storage As Dictionary(Of Guid, Technology)
		Shared Sub New()
			storage = New Dictionary(Of Guid, Technology)()
		End Sub
		Public Sub New(ByVal npos As NonPersistentObjectSpace)
			MyBase.New(npos)
		End Sub
		Protected Overrides Function LoadObjectByKey(ByVal key As Guid) As Technology
			Dim result As Technology = Nothing
			Dim objData As Technology = Nothing
			If storage.TryGetValue(key, objData) Then
				result = New Technology(key)
				result.Assign(objData)
			End If
			Return result
		End Function
		Protected Overrides Function ReloadObject(ByVal obj As Technology) As Technology
			Dim objData As Technology = LoadData(obj.Oid)
			obj.Assign(objData)
			Return obj
		End Function
		Protected Overrides Sub CommitChanges(ByVal objects As List(Of Technology))
			For Each obj In objects
				Dim objData As Technology
				If ObjectSpace.IsDeletedObject(obj) Then
					storage.Remove(obj.Oid)
				ElseIf ObjectSpace.IsNewObject(obj) Then
					objData = New Technology(obj.Oid)
					objData.Assign(obj)
					storage.Add(obj.Oid, objData)
				Else
					objData = LoadData(obj.Oid)
					objData.Assign(obj)
				End If
			Next obj
		End Sub
		Private Function LoadData(ByVal key As Guid) As Technology
			Dim objData As Technology = Nothing
			If Not storage.TryGetValue(key, objData) Then
				Throw New InvalidOperationException("Object is not found in the storage.")
			End If
			Return objData
		End Function
	End Class
End Namespace
