Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports DevExpress.ExpressApp
Imports DevExpress.ExpressApp.DC

Namespace NonPersistentObjectsDemo.Module.BusinessObjects

	<DomainComponent>
	<DefaultProperty(NameOf(Group.Name))>
	Public Class Group
		<DevExpress.ExpressApp.Data.Key>
		Public Property Name() As String
	End Class

	Friend Class NPGroupAdapter
		Private _objectSpace As NonPersistentObjectSpace
		Protected ReadOnly Property ObjectSpace() As NonPersistentObjectSpace
			Get
				Return _objectSpace
			End Get
		End Property
		Private objects As List(Of Group)
		Public Sub New(ByVal npos As NonPersistentObjectSpace)
			Me._objectSpace = npos
			AddHandler _objectSpace.ObjectsGetting, AddressOf ObjectSpace_ObjectsGetting
			AddHandler _objectSpace.ObjectByKeyGetting, AddressOf ObjectSpace_ObjectByKeyGetting
		End Sub
		Protected Function GetObjectByKey(ByVal key As String) As Group
			Return New Group() With {.Name = key}
		End Function
		Private Sub ObjectSpace_ObjectByKeyGetting(ByVal sender As Object, ByVal e As ObjectByKeyGettingEventArgs)
			If e.Key IsNot Nothing Then
				If e.ObjectType Is GetType(Group) Then
					e.Object = GetObjectByKey(CStr(e.Key))
				End If
			End If
		End Sub
		Private Sub ObjectSpace_ObjectsGetting(ByVal sender As Object, ByVal e As ObjectsGettingEventArgs)
			If e.ObjectType Is GetType(Group) Then
				If objects Is Nothing Then
					Dim pos = TryCast(ObjectSpace.Owner, IObjectSpace)
					objects = pos.GetObjectsQuery(Of Product)().Where(Function(o) o.GroupName IsNot Nothing).GroupBy(Function(o) o.GroupName).Select(Function(o) GetObjectByKey(o.Key)).ToList()
				End If
				e.Objects = objects
			End If
		End Sub
	End Class
End Namespace
