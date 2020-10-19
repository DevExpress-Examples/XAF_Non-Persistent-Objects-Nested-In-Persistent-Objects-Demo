Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Xml.Serialization
Imports DevExpress.Data.Filtering
Imports DevExpress.ExpressApp
Imports DevExpress.Persistent.Base
Imports DevExpress.Persistent.BaseImpl
Imports DevExpress.Xpo

Namespace NonPersistentObjectsDemo.Module.BusinessObjects

	<DevExpress.ExpressApp.DC.XafDefaultProperty(NameOf(Project.CodeName))>
	<DefaultClassOptions>
	Public Class Project
		Inherits BaseObject
		Implements IObjectSpaceLink

		Public Sub New(ByVal session As Session)
			MyBase.New(session)
		End Sub

		Private _CodeName As String
		Public Property CodeName() As String
			Get
				Return _CodeName
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)(NameOf(CodeName), _CodeName, value)
			End Set
		End Property

		#Region "Features"
		Private _Features As BindingList(Of Feature)
		<Aggregated>
		Public ReadOnly Property Features() As BindingList(Of Feature)
			Get
				If _Features Is Nothing Then
					_Features = New BindingList(Of Feature)()
					AddHandler _Features.ListChanged, AddressOf _Features_ListChanged
				End If
				Return _Features
			End Get
		End Property
		Private Sub _Features_ListChanged(ByVal sender As Object, ByVal e As ListChangedEventArgs)
			Dim list = DirectCast(sender, BindingList(Of Feature))
			If e.ListChangedType = ListChangedType.ItemAdded Then
				Dim obj = list(e.NewIndex)
				obj.OwnerKey = Me.Oid
				obj.LocalKey = e.NewIndex + 1
			End If
			FeatureList = SerializationHelper.Save(Features)
		End Sub
		Private _FeatureList As String
		<Browsable(False)>
		<Size(SizeAttribute.Unlimited)>
		Public Property FeatureList() As String
			Get
				Return _FeatureList
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)(NameOf(FeatureList), _FeatureList, value)
			End Set
		End Property
		#End Region

		#Region "MainFeature"
		Private _MainFeature As Feature
		<DataSourceProperty(NameOf(Features))>
		Public Property MainFeature() As Feature
			Get
				Return _MainFeature
			End Get
			Set(ByVal value As Feature)
				SetPropertyValue(Of Feature)(NameOf(MainFeature), _MainFeature, value)
			End Set
		End Property
		Private _MainFeatureName As String
		<Browsable(False)>
		Public Property MainFeatureName() As String
			Get
				Return _MainFeatureName
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)(NameOf(MainFeatureName), _MainFeatureName, value)
			End Set
		End Property
		#End Region

		#Region "Resources"
		Private _Resources As BindingList(Of Resource)
		<Aggregated>
		Public ReadOnly Property Resources() As BindingList(Of Resource)
			Get
				If _Resources Is Nothing Then
					_Resources = New BindingList(Of Resource)()
					AddHandler _Resources.ListChanged, AddressOf _Resources_ListChanged
				End If
				Return _Resources
			End Get
		End Property
		Private Sub _Resources_ListChanged(ByVal sender As Object, ByVal e As ListChangedEventArgs)
			Dim list = DirectCast(sender, BindingList(Of Resource))
			If e.ListChangedType = ListChangedType.ItemAdded Then
				list(e.NewIndex).OwnerKey = Me.Oid
			End If
			ResourceList = SerializationHelper.Save(Resources)
		End Sub
		Private _ResourceList As String
		<Browsable(False)>
		<Size(SizeAttribute.Unlimited)>
		Public Property ResourceList() As String
			Get
				Return _ResourceList
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)(NameOf(ResourceList), _ResourceList, value)
			End Set
		End Property
		#End Region

		Protected Overrides Sub OnChanged(ByVal propertyName As String, ByVal oldValue As Object, ByVal newValue As Object)
			MyBase.OnChanged(propertyName, oldValue, newValue)
			If propertyName = NameOf(MainFeature) Then
				MainFeatureName = TryCast(newValue, Feature)?.Name
			End If
		End Sub
		Protected Overrides Sub OnLoaded()
			MyBase.OnLoaded()
			Dim counter As Integer = 0
			SerializationHelper.Load(Features, FeatureList, ObjectSpace, Sub(o)
				o.OwnerKey = Me.Oid
				counter += 1
				o.LocalKey = counter
			End Sub)
			SerializationHelper.Load(Resources, ResourceList, ObjectSpace, Sub(o)
				o.OwnerKey = Me.Oid
			End Sub)
			_MainFeature = If(MainFeatureName Is Nothing, Nothing, Features.FirstOrDefault(Function(f) f.Name = MainFeatureName))
		End Sub
		Protected Overrides Sub OnSaving()
			FeatureList = SerializationHelper.Save(Features)
			ResourceList = SerializationHelper.Save(Resources)
			MyBase.OnSaving()
		End Sub
		Private _objectSpace As IObjectSpace
		Protected ReadOnly Property ObjectSpace() As IObjectSpace
			Get
				Return _objectSpace
			End Get
		End Property
		Private Property IObjectSpaceLink_ObjectSpace() As IObjectSpace Implements IObjectSpaceLink.ObjectSpace
			Get
				Return _objectSpace
			End Get
			Set(ByVal value As IObjectSpace)
				If _objectSpace IsNot value Then
					_objectSpace = value
				End If
			End Set
		End Property
	End Class
End Namespace
