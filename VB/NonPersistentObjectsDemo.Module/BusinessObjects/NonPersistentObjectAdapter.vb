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

	Public Interface IAssignable(Of T)
		Sub Assign(ByVal source As T)
	End Interface

	Public MustInherit Class NonPersistentObjectAdapter(Of TObject, TKey)
		Private _objectSpace As NonPersistentObjectSpace
		Private objectMap As Dictionary(Of TKey, TObject)
		Protected ReadOnly Property ObjectSpace() As NonPersistentObjectSpace
			Get
				Return _objectSpace
			End Get
		End Property
		Public Sub New(ByVal npos As NonPersistentObjectSpace)
			Me._objectSpace = npos
			AddHandler _objectSpace.ObjectsGetting, AddressOf ObjectSpace_ObjectsGetting
			AddHandler _objectSpace.ObjectGetting, AddressOf ObjectSpace_ObjectGetting
			AddHandler _objectSpace.ObjectByKeyGetting, AddressOf ObjectSpace_ObjectByKeyGetting
			AddHandler _objectSpace.Reloaded, AddressOf ObjectSpace_Reloaded
			AddHandler _objectSpace.ObjectReloading, AddressOf ObjectSpace_ObjectReloading
			AddHandler _objectSpace.CustomCommitChanges, AddressOf ObjectSpace_CustomCommitChanges
			objectMap = New Dictionary(Of TKey, TObject)()
		End Sub
		Protected Overridable Sub GuardKeyNotEmpty(ByVal obj As TObject)
			If Object.Equals(GetKeyValue(obj), CType(Nothing, TKey)) Then
				Throw New InvalidOperationException() ' DEBUG
			End If
		End Sub
		Protected Overridable Function GetKeyValue(ByVal obj As TObject) As TKey
			Return CType(_objectSpace.GetKeyValue(obj), TKey)
		End Function
		Private Sub AcceptObject(ByVal obj As TObject)
			Dim result As TObject = Nothing
			GuardKeyNotEmpty(obj)
			If Not objectMap.TryGetValue(GetKeyValue(obj), result) Then
				objectMap.Add(GetKeyValue(obj), obj)
			Else
				If Not Object.Equals(result, obj) Then
					Throw New InvalidOperationException()
				End If
			End If
		End Sub
		Protected Function GetObjectByKey(ByVal key As TKey) As TObject
			Dim result As TObject = Nothing
			If Not objectMap.TryGetValue(key, result) Then
				result = LoadObjectByKey(key)
				If result IsNot Nothing Then
					AcceptObject(result)
				End If
			End If
			Return result
		End Function
		Protected Overridable Function LoadObjectByKey(ByVal key As TKey) As TObject
			Throw New NotSupportedException()
		End Function
		Private Sub ObjectSpace_ObjectByKeyGetting(ByVal sender As Object, ByVal e As ObjectByKeyGettingEventArgs)
			If e.Key IsNot Nothing Then
				If e.ObjectType Is GetType(TObject) Then
					e.Object = GetObjectByKey(CType(e.Key, TKey))
				End If
			End If
		End Sub
		Private Sub ObjectSpace_ObjectsGetting(ByVal sender As Object, ByVal e As ObjectsGettingEventArgs)
			If e.ObjectType Is GetType(TObject) Then
				Dim objects = GetObjects()
				e.Objects = DirectCast(objects, System.Collections.IList)
			End If
		End Sub
		Protected Overridable Function GetObjects() As IList(Of TObject)
			Throw New NotSupportedException()
		End Function
		Private Sub ObjectSpace_Reloaded(ByVal sender As Object, ByVal e As EventArgs)
			objectMap.Clear()
		End Sub
		Private Sub ObjectSpace_ObjectGetting(ByVal sender As Object, ByVal e As ObjectGettingEventArgs)
			If TypeOf e.SourceObject Is TObject Then
				Dim obj = CType(e.SourceObject, TObject)
				Dim link = TryCast(e.SourceObject, IObjectSpaceLink)
				If link IsNot Nothing Then
					GuardKeyNotEmpty(obj)
					If link.ObjectSpace Is Nothing Then
						e.TargetObject = AcceptOrUpdate(obj)
					Else
						If link.ObjectSpace.IsNewObject(obj) Then
							If link.ObjectSpace.Equals(_objectSpace) Then
								e.TargetObject = e.SourceObject
							Else
								e.TargetObject = Nothing
							End If
						Else
							If link.ObjectSpace.Equals(_objectSpace) Then
								e.TargetObject = AcceptOrUpdate(obj)
							Else
								Dim result As TObject = Nothing
								If Not objectMap.TryGetValue(GetKeyValue(obj), result) Then
									result = LoadSameObject(obj)
									If result IsNot Nothing Then
										AcceptObject(result)
									End If
								End If
								e.TargetObject = result
							End If
						End If
					End If
				End If
			End If
		End Sub
		Protected Overridable ReadOnly Property ThrowOnAcceptingMismatchedObject() As Boolean
			Get
				Return False
			End Get
		End Property
		Private Function AcceptOrUpdate(ByVal obj As TObject) As TObject
			Dim key = GetKeyValue(obj)
			Dim result As TObject = Nothing
			If Not objectMap.TryGetValue(key, result) Then
				objectMap.Add(key, obj)
				result = obj
			Else
				' if objectMap contains an object with the same key, assume SourceObject is a reloaded copy.
				' then refresh contents of the found object and return it.
				If Not Object.Equals(result, obj) Then
					If ThrowOnAcceptingMismatchedObject Then
						Throw New InvalidOperationException()
					End If
					Dim tempVar As Boolean = TypeOf result Is IAssignable(Of TObject)
					Dim a As IAssignable(Of TObject) = If(tempVar, CType(result, IAssignable(Of TObject)), Nothing)
					If tempVar Then
						a.Assign(obj)
					End If
				End If
			End If
			Return result
		End Function
		Protected Overridable Function LoadSameObject(ByVal obj As TObject) As TObject
			Return LoadObjectByKey(GetKeyValue(obj))
		End Function
		Private Sub ObjectSpace_ObjectReloading(ByVal sender As Object, ByVal e As ObjectGettingEventArgs)
			If TypeOf e.SourceObject Is TObject Then
				Dim tobj = CType(e.SourceObject, TObject)
				e.TargetObject = ReloadObject(tobj)
			End If
		End Sub
		Protected Overridable Function ReloadObject(ByVal obj As TObject) As TObject
			Return obj
		End Function
		Private Sub ObjectSpace_CustomCommitChanges(ByVal sender As Object, ByVal e As HandledEventArgs)
			Dim list As New List(Of TObject)()
			For Each obj In _objectSpace.ModifiedObjects
				If TypeOf obj Is TObject Then
					list.Add(CType(obj, TObject))
				End If
			Next obj
			If list.Count > 0 Then
				CommitChanges(list)
			End If
		End Sub
		Protected Overridable Sub CommitChanges(ByVal objects As List(Of TObject))
		End Sub
	End Class

End Namespace
