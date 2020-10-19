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
	Public Class Department
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

		#Region "Agents"
		Private _Agents As BindingList(Of Agent)
		<Aggregated>
		Public ReadOnly Property Agents() As BindingList(Of Agent)
			Get
				If _Agents Is Nothing Then
					_Agents = New BindingList(Of Agent)()
					AddHandler _Agents.ListChanged, AddressOf _Agents_ListChanged
				End If
				Return _Agents
			End Get
		End Property
		Private Sub _Agents_ListChanged(ByVal sender As Object, ByVal e As ListChangedEventArgs)
			Dim list = DirectCast(sender, BindingList(Of Agent))
			If e.ListChangedType = ListChangedType.ItemAdded Then
				Agent.Sequence += 1
'INSTANT VB WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: list[e.NewIndex].ID = ++Agent.Sequence;
				list(e.NewIndex).ID = Agent.Sequence
			End If
			AgentList = SerializationHelper.Save(Agents)
		End Sub
		Private _AgentList As String
		<Browsable(False)>
		<Size(SizeAttribute.Unlimited)>
		Public Property AgentList() As String
			Get
				Return _AgentList
			End Get
			Set(ByVal value As String)
				SetPropertyValue(Of String)(NameOf(AgentList), _AgentList, value)
			End Set
		End Property
		#End Region

		Protected Overrides Sub OnLoaded()
			MyBase.OnLoaded()
			SerializationHelper.Load(Agents, AgentList, ObjectSpace, Sub(o)
				Agent.Sequence += 1
				o.ID = Agent.Sequence
			End Sub)
		End Sub
		Protected Overrides Sub OnSaving()
			AgentList = SerializationHelper.Save(Agents)
			MyBase.OnSaving()
		End Sub
'INSTANT VB NOTE: The field objectSpace was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private objectSpace_Conflict As IObjectSpace
		Protected ReadOnly Property ObjectSpace() As IObjectSpace
			Get
				Return objectSpace_Conflict
			End Get
		End Property
		Private Property IObjectSpaceLink_ObjectSpace() As IObjectSpace Implements IObjectSpaceLink.ObjectSpace
			Get
				Return objectSpace_Conflict
			End Get
			Set(ByVal value As IObjectSpace)
				If objectSpace_Conflict IsNot value Then
					objectSpace_Conflict = value
				End If
			End Set
		End Property
	End Class
End Namespace
