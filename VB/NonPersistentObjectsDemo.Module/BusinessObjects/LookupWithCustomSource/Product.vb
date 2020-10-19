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

	<DevExpress.ExpressApp.DC.XafDefaultProperty(NameOf(Name))>
	<DefaultClassOptions>
	Public Class Product
		Inherits BaseObject

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
		Private _Group As Group
		Public Property Group() As Group
			Get
				Return _Group
			End Get
			Set(ByVal value As Group)
				SetPropertyValue(Of Group)(NameOf(Group), _Group, value)
			End Set
		End Property
		Private _GroupName As String
		<Browsable(False)>
		Public Property GroupName() As String
			Get
				Return _GroupName
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)(NameOf(GroupName), _GroupName, value)
			End Set
		End Property

		Protected Overrides Sub OnChanged(ByVal propertyName As String, ByVal oldValue As Object, ByVal newValue As Object)
			MyBase.OnChanged(propertyName, oldValue, newValue)
			If propertyName = NameOf(Group) Then
				GroupName = TryCast(newValue, Group)?.Name
			End If
		End Sub
		Protected Overrides Sub OnLoaded()
			MyBase.OnLoaded()
			_Group = If(GroupName Is Nothing, Nothing, New Group() With {.Name = GroupName})
		End Sub
	End Class
End Namespace
