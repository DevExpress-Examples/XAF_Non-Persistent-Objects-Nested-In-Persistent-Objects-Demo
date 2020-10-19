Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Xml.Serialization
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC
Imports DevExpress.ExpressApp.SystemModule
Imports DevExpress.Persistent.Base

Namespace NonPersistentObjectsDemo.Module.BusinessObjects

	<ListViewFilter("All", "", True)>
	<ListViewFilter("None", "1=0")>
	<ListViewFilter("Started", "Progress > 0")>
	<DomainComponent>
	Public Class Feature
		Inherits NonPersistentObjectImpl
		Implements IAssignable(Of Feature)

		Public Sub New()
			MyBase.New()
		End Sub
		<Browsable(False)>
		<XmlIgnore>
		Public Property LocalKey() As Integer
		<Browsable(False)>
		<XmlIgnore>
		Public Property OwnerKey() As Guid
		<VisibleInDetailView(False)>
		<VisibleInListView(False)>
		<DevExpress.ExpressApp.Data.Key>
		Public ReadOnly Property ID() As String
			Get
				Return String.Format("{0:N}.{1:x8}", OwnerKey, LocalKey)
			End Get
		End Property
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
		Public Sub Assign(ByVal source As Feature) Implements IAssignable(Of Feature).Assign
			OwnerKey = source.OwnerKey
			LocalKey = source.LocalKey
			Name = source.Name
			Progress = source.Progress
		End Sub
	End Class

	Friend Class NPFeatureAdapter
		Inherits NonPersistentObjectAdapter(Of Feature, String)

		Public Sub New(ByVal npos As NonPersistentObjectSpace)
			MyBase.New(npos)
		End Sub
		Protected Overrides Sub GuardKeyNotEmpty(ByVal obj As Feature)
			If obj.OwnerKey = Guid.Empty Then
				Throw New InvalidOperationException() ' DEBUG
			End If
			If obj.LocalKey = 0 Then
				Throw New InvalidOperationException() ' DEBUG
			End If
		End Sub
		Protected Overrides Function LoadObjectByKey(ByVal key As String) As Feature
			Dim result As Feature
			Dim ownerKey As Guid = Nothing
			Dim localKey As Integer = Nothing
			If Not TryParseKey(key, ownerKey, localKey) Then
				Throw New InvalidOperationException("Invalid key.")
			End If
			Dim owner = GetOwnerByKey(ownerKey)
			result = GetFromOwner(owner, localKey)
			If result Is Nothing Then
				owner = ReloadOwner(owner)
				result = GetFromOwner(owner, localKey)
			End If
			Return result
		End Function
		Private Function GetFromOwner(ByVal owner As Project, ByVal localKey As Integer) As Feature
			If owner Is Nothing Then
				Throw New InvalidOperationException("Owner object is not found in the storage.")
			End If
			Return owner.Features.FirstOrDefault(Function(o) o.LocalKey = localKey)
		End Function
		Private Function GetOwnerByKey(ByVal key As Guid) As Project
			Dim ownerObjectSpace = TryCast(ObjectSpace.Owner, CompositeObjectSpace)
			Return (If(ownerObjectSpace, ObjectSpace)).GetObjectByKey(Of Project)(key)
		End Function
		Private Function ReloadOwner(ByVal owner As Project) As Project
			Dim ownerObjectSpace = TryCast(ObjectSpace.Owner, CompositeObjectSpace)
			If ownerObjectSpace.ModifiedObjects.Contains(owner) Then
				Throw New NotSupportedException()
			End If
			Return CType((If(ownerObjectSpace, ObjectSpace)).ReloadObject(owner), Project)
		End Function
		Private Function TryParseKey(ByVal key As String, ByRef ownerKey As Guid, ByRef localKey As Integer) As Boolean
			ownerKey = Guid.Empty
			localKey = 0
			If String.IsNullOrEmpty(key) Then
				Return False
			End If
			Dim parts = key.Split("."c)
			If parts.Length <> 2 Then
				Return False
			End If
			If Not Guid.TryParse(parts(0), ownerKey) Then
				Return False
			End If
			If Not Int32.TryParse(parts(1), localKey) Then
				Return False
			End If
			Return True
		End Function
	End Class
End Namespace
