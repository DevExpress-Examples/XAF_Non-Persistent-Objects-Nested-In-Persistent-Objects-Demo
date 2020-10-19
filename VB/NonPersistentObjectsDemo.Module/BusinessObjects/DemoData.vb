Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports DevExpress.ExpressApp

Namespace NonPersistentObjectsDemo.Module.BusinessObjects

	Friend Class DemoDataCreator
		Private ObjectSpace As IObjectSpace
		Public Sub New(ByVal objectSpace As IObjectSpace)
			Me.ObjectSpace = objectSpace
		End Sub
		Public Sub CreateDemoObjects()
			If ObjectSpace.CanInstantiate(GetType(Project)) Then
				If Not ObjectSpace.CanInstantiate(GetType(Technology)) Then
					Dim typesInfo = ObjectSpace.TypesInfo
					Dim npos = New NonPersistentObjectSpace(typesInfo, CType(typesInfo, DevExpress.ExpressApp.DC.TypesInfo).FindEntityStore(GetType(DevExpress.ExpressApp.DC.NonPersistentTypeInfoSource)))
					DirectCast(ObjectSpace, CompositeObjectSpace).AdditionalObjectSpaces.Add(npos)
					DirectCast(ObjectSpace, CompositeObjectSpace).AutoCommitAdditionalObjectSpaces = True
					Dim tempVar As New NPTechnologyAdapter(npos)
				End If
				CreateProjects()
				CreateProducts()
				CreateEpochs()
				CreateDepartments()
			End If
		End Sub
		Private Sub CreateProjects()
			Dim p1 = CreateProject("Project X")
			Dim p2 = CreateProject("Project Y")
			p2.Features.Add(New Feature() With {
				.Name = "Feature 1",
				.Progress = 3.5
			})
			p2.Features.Add(New Feature() With {
				.Name = "Feature 2",
				.Progress = 0
			})
			p2.Features.Add(New Feature() With {
				.Name = "Feature 3",
				.Progress = 1
			})
			p2.Resources.Add(New Resource() With {
				.Name = "Resource A",
				.URI = "a",
				.Embed = True
			})
			p2.Resources.Add(New Resource() With {
				.Name = "Resource B",
				.URI = "b",
				.Priority = 2
			})
			p2.Resources.Add(New Resource() With {
				.Name = "Resource C",
				.URI = "c",
				.Priority = 1
			})
			Dim p3 = CreateProject("Project Z")
		End Sub
		Private Function CreateProject(ByVal name As String) As Project
			Dim project = ObjectSpace.CreateObject(Of Project)()
			project.CodeName = name
			Return project
		End Function
		Private Sub CreateProducts()
			Dim p1 = CreateProduct("Product 1", "B")
			Dim p2 = CreateProduct("Product 2", "B")
			Dim p4 = CreateProduct("Product 3", "XHD")
			Dim p3 = CreateProduct("Product 4", "AAA")
		End Sub
		Private Function CreateProduct(ByVal name As String, ByVal group As String) As Product
			Dim project = ObjectSpace.CreateObject(Of Product)()
			project.Name = name
			project.Group = New Group() With {.Name = group}
			Return project
		End Function
		Private Sub CreateEpochs()
			Dim t1 = CreateTechnology("Tech 1", "Technology 1")
			Dim t2 = CreateTechnology("Tech 2", "Technology 2")
			Dim t3 = CreateTechnology("Tech 3", "Technology 3")
			Dim e1 = CreateEpoch("Stone Age")
			Dim e2 = CreateEpoch("Nowadays")
			Dim e3 = CreateEpoch("Future")
			e2.Technologies.Add(t1)
			e2.Technologies.Add(t2)
			e2.Technologies.Add(t3)
		End Sub
		Private Function CreateEpoch(ByVal name As String) As Epoch
			Dim obj = ObjectSpace.CreateObject(Of Epoch)()
			obj.Name = name
			Return obj
		End Function
		Private Function CreateTechnology(ByVal name As String, ByVal description As String) As Technology
			Dim tech = ObjectSpace.CreateObject(Of Technology)()
			tech.Name = name
			tech.Description = description
			Return tech
		End Function
		Private Sub CreateDepartments()
			Dim e1 = CreateDepartment("Sales")
			Dim e2 = CreateDepartment("Research")
			Dim e3 = CreateDepartment("Communications")
			e2.Agents.Add(New Agent() With {
				.Name = "Agent X",
				.Progress = 80
			})
			e2.Agents.Add(New Agent() With {
				.Name = "Agent Orange",
				.Progress = 0
			})
		End Sub
		Private Function CreateDepartment(ByVal name As String) As Department
			Dim obj = ObjectSpace.CreateObject(Of Department)()
			obj.Name = name
			Return obj
		End Function
	End Class
End Namespace
