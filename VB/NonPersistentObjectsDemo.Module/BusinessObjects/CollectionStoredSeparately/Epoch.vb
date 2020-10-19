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

	<DevExpress.ExpressApp.DC.XafDefaultProperty(NameOf(Epoch.Name))>
	<DefaultClassOptions>
	Public Class Epoch
		Inherits BaseObject
		Implements IObjectSpaceLink

		Public Sub New(ByVal session As Session)
			MyBase.New(session)
		End Sub

		Private _Name As String
		Public Property Name() As String
			Get
				Return _Name
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)(NameOf(Name), _Name, value)
			End Set
		End Property

		#Region "Technologies"
		Private _Technologies As BindingList(Of Technology)
		Public ReadOnly Property Technologies() As BindingList(Of Technology)
			Get
				If _Technologies Is Nothing Then
					_Technologies = New BindingList(Of Technology)()
				End If
				Return _Technologies
			End Get
		End Property
		Private _TechnologyList As String
		<Browsable(False)>
		<Size(SizeAttribute.Unlimited)>
		Public Property TechnologyList() As String
			Get
				Return _TechnologyList
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)(NameOf(TechnologyList), _TechnologyList, value)
			End Set
		End Property
		#End Region

		Protected Overrides Sub OnLoaded()
			MyBase.OnLoaded()
			LoadList(Technologies, TechnologyList)
		End Sub
		Protected Overrides Sub OnSaving()
			TechnologyList = SaveList(Technologies)
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

		#Region "NP Serialization"
		Private Sub LoadList(ByVal list As IList(Of Technology), ByVal data As String)
			list.Clear()
			If data IsNot Nothing Then
				For Each s In data.Split(","c)
					Dim key As Guid = Nothing
					If Guid.TryParse(s, key) Then
						Dim obj = ObjectSpace.GetObjectByKey(Of Technology)(key)
						If obj IsNot Nothing Then
							list.Add(obj)
						End If
					End If
				Next s
			End If
		End Sub
		Private Function SaveList(ByVal list As IList(Of Technology)) As String
			If list Is Nothing OrElse list.Count = 0 Then
				Return Nothing
			End If
			Return String.Join(",", list.Select(Function(o) o.Oid.ToString("D")))
		End Function
		#End Region
	End Class
End Namespace
