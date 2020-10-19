Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Xml.Serialization
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.Persistent.Base

Namespace NonPersistentObjectsDemo.Module.BusinessObjects

	<DomainComponent>
	Public Class Resource
		Inherits NonPersistentObjectImpl
		Implements IAssignable(Of Resource)

		<VisibleInDetailView(False)>
		<VisibleInListView(False)>
		<DevExpress.ExpressApp.Data.Key>
		Public Property Oid() As Guid
		Private _Name As String
		Public Property Name() As String
			Get
				Return _Name
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)(_Name, value)
			End Set
		End Property
		Private _URI As String
		Public Property URI() As String
			Get
				Return _URI
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)(_URI, value)
			End Set
		End Property
		Private _Priority As Integer
		Public Property Priority() As Integer
			Get
				Return _Priority
			End Get
			Set(ByVal value As Integer)
				SetPropertyValue(Of Integer)(_Priority, value)
			End Set
		End Property
		Private _Embed As Boolean
		Public Property Embed() As Boolean
			Get
				Return _Embed
			End Get
			Set(ByVal value As Boolean)
				SetPropertyValue(Of Boolean)(_Embed, value)
			End Set
		End Property
		<Browsable(False)>
		<XmlIgnore>
		Public Property OwnerKey() As Guid
		Public Sub Assign(ByVal source As Resource) Implements IAssignable(Of Resource).Assign
			Oid = source.Oid
			OwnerKey = source.OwnerKey
			Name = source.Name
			URI = source.URI
			Priority = source.Priority
			Embed = source.Embed
		End Sub
		Public Sub New()
			Oid = Guid.NewGuid()
		End Sub
	End Class

	Friend Class NPResourceAdapter
		Inherits NonPersistentObjectAdapter(Of Resource, Guid)

		Public Sub New(ByVal npos As NonPersistentObjectSpace)
			MyBase.New(npos)
		End Sub
		Protected Overrides Sub GuardKeyNotEmpty(ByVal obj As Resource)
			If obj.OwnerKey = Guid.Empty Then
				Throw New InvalidOperationException() ' DEBUG
			End If
			If obj.Oid = Guid.Empty Then
				Throw New InvalidOperationException() ' DEBUG
			End If
		End Sub
		Protected Overrides Function LoadSameObject(ByVal obj As Resource) As Resource
			Dim result As Resource
			Dim owner = GetOwnerByKey(obj.OwnerKey)
			result = GetFromOwner(owner, obj.Oid)
			If result Is Nothing Then
				owner = ReloadOwner(owner)
				result = GetFromOwner(owner, obj.Oid)
			End If
			Return result
		End Function
		Private Function GetFromOwner(ByVal owner As Project, ByVal localKey As Guid) As Resource
			If owner Is Nothing Then
				Throw New InvalidOperationException("Owner object is not found in the storage.")
			End If
			Return owner.Resources.FirstOrDefault(Function(o) o.Oid = localKey)
		End Function
		Private Function GetOwnerByKey(ByVal key As Guid) As Project
			Dim ownerObjectSpace = TryCast(ObjectSpace.Owner, CompositeObjectSpace)
			Return (If(ownerObjectSpace, ObjectSpace)).GetObjectByKey(Of Project)(key)
		End Function
		Private Function ReloadOwner(ByVal owner As Project) As Project
			Dim ownerObjectSpace = TryCast(ObjectSpace.Owner, CompositeObjectSpace)
			Return CType((If(ownerObjectSpace, ObjectSpace)).ReloadObject(owner), Project)
		End Function
	End Class
End Namespace
