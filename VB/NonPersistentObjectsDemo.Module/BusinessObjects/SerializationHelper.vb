Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Xml.Serialization
Imports DevExpress.ExpressApp

Namespace NonPersistentObjectsDemo.Module.BusinessObjects

	Friend Module SerializationHelper
		Public Sub Load(Of T)(ByVal list As BindingList(Of T), ByVal data As String, ByVal objectSpace As IObjectSpace, ByVal acceptor As Action(Of T))
			list.RaiseListChangedEvents = False
			list.Clear()
			If data IsNot Nothing Then
				Dim serializer = New XmlSerializer(GetType(T).MakeArrayType())
				Using stream = New MemoryStream(Encoding.UTF8.GetBytes(data))
					Dim objs = TryCast(serializer.Deserialize(stream), IList(Of T))
					For Each obj In objs
						acceptor?.Invoke(obj)
						Dim tobj = objectSpace.GetObject(obj)
						Dim aobj = TryCast(tobj, IAssignable(Of T))
						If aobj IsNot Nothing Then
							aobj.Assign(obj)
						End If
						list.Add(tobj)
					Next obj
				End Using
			End If
			list.RaiseListChangedEvents = True
			list.ResetBindings()
		End Sub
		Public Function Save(Of T)(ByVal list As IList(Of T)) As String
			If list Is Nothing OrElse list.Count = 0 Then
				Return Nothing
			End If
			Dim serializer = New XmlSerializer(GetType(T).MakeArrayType())
			Using stream = New MemoryStream()
				serializer.Serialize(stream, list.ToArray())
				Return Encoding.UTF8.GetString(stream.ToArray())
			End Using
		End Function
	End Module
End Namespace
