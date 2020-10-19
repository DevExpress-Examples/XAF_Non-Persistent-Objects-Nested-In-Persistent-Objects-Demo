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
	<DefaultProperty(NameOf(Name))>
	Public Class Group
		<DevExpress.ExpressApp.Data.Key>
		Public Property Name() As String
	End Class

	Friend Class NPGroupAdapter
'INSTANT VB NOTE: The field objectSpace was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private objectSpace_Conflict As NonPersistentObjectSpace
		Protected ReadOnly Property ObjectSpace() As NonPersistentObjectSpace
			Get
				Return objectSpace_Conflict
			End Get
		End Property
		Private objects As List(Of Group)
		Public Sub New(ByVal npos As NonPersistentObjectSpace)
			Me.objectSpace_Conflict = npos
			AddHandler objectSpace_Conflict.ObjectsGetting, AddressOf ObjectSpace_ObjectsGetting
			AddHandler objectSpace_Conflict.ObjectByKeyGetting, AddressOf ObjectSpace_ObjectByKeyGetting
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
