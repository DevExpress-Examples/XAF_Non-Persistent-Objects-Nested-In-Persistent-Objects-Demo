<!-- default badges list -->
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T919644)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->

# How to edit Non-Persistent Objects nested in a Persistent Object



It is often required to store some complex data in persistent business objects in a compact form (as a string or a byte array) but show and edit this complex data as objects using the standard XAF UI. To solve this task in XAF, use [Non\-Persistent Objects](https://docs.devexpress.com/eXpressAppFramework/116516/concepts/business-model-design/non-persistent-objects) nested in persistent business objects as reference or collection properties. This example demonstrates possible implementations for a few such scenarios.

To make certain built-in functionality work for the combination of persistent and non-persistent objects, in the common Module we subscribe to the [XafApplication\.ObjectSpaceCreated](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.XafApplication.ObjectSpaceCreated) event and call the **CompositeObjectSpace.PopulateAdditionalObjectSpaces** method. Also, we enable the **AutoCommitAdditionalObjectSpaces**, **AutoRefreshAdditionalObjectSpaces**, and [AutoSetModifiedOnObjectChange](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.AutoSetModifiedOnObjectChange) options and setup helpers (adapters) that will handle [NonPersistentObjectSpace](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace) events.

> [!WARNING]
> We created this example for demonstration purposes and didn't intend to address all possible usage scenarios with it.
> If this example doesn't have some functionality or you wish to change its behavior, feel free to extend this example as you need. Please note that this is quite a complex task, which requires good knowledge of XAF: [UI Customization Categories by Skill Level](https://www.devexpress.com/products/net/application_framework/xaf-considerations-for-newcomers.xml#ui-customization-categories). You will likely need to research how our components work under the hood. Refer to the following help topic for more information: [Debug DevExpress .NET Source Code with PDB Symbols](https://docs.devexpress.com/GeneralInformation/403656/support-debug-troubleshooting/debug-controls-with-debug-symbols).
> Unfortunately, we can't help you much with such tasks as custom programming goes beyond our Support Service scope: [Technical Support Scope](https://www.devexpress.com/products/net/application_framework/xaf-considerations-for-newcomers.xml#support).


## Implementation Details

### Scenario 1: A Non-Persistent lookup property

In a persistent business object, we have a string field. We want to represent this field in the UI using a lookup editor, so a user can select from existing values of add a new value. The list of existing values is generated dynamically.

This scenario is demonstrated by the **Product** business object. We add a hidden persistent *GroupName* string property and a visible non-persistent *Group* property. The non-persistent **Group** class represents existing string values and has the *Name* property that is also a key property. In the *Product* class, we override the *OnLoaded* method to create a *Group* based on the stored *GroupName* value. We also override the *OnChanged* method to update the *GroupName* property when the *Group* property is changed. To populate the lookup list view, we subscribe to the [NonPersistentObjectSpace\.ObjectsGetting](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.ObjectsGetting) event and collect distinct group names from all existing Product objects.

*Files to look at*:
* [Product.cs](./CS/XPO/NonPersistentDemo/NonPersistentDemo.Module/BusinessObjects/LookupWithCustomSource/Product.cs)
* [Group.cs](./CS/XPO/NonPersistentDemo/NonPersistentDemo.Module/BusinessObjects/LookupWithCustomSource/Group.cs)


### Scenario 2: A nested collection of Non-Persistent objects stored in the owner Persistent object

In a persistent business object, we have a string field where we store a collection of complex data items serialized to XML. We want to show this XML in the UI as a nested list view and allow users to edit collection items, add new items, and delete existing items.

#### Solution A

This solution is demonstrated by the **Project** business object. The non-persistent **Feature** class represents complex collection items. The *Feature* class has a compound key that consists of the *OwnerKey* and *LocalKey* parts. The *OwnerKey* is used to locate the owner object (*Project*). The *LocalKey* is used to identify a *Feature* object within the collection. These keys are not serialized and exist at runtime only. 

In the *Project* class, we have a hidden persistent *FeatureList* string property and a visible non-persistent *Features* aggregated collection property. We override the *OnLoaded* and *OnSaving* methods to serialize and deserialize the *Features* collection. Note that after deserialization we initialize the local key property and the owner key property. Then, we call the [NonPersistentObjectSpace\.GetObject](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.GetObject(System.Object)) method to avoid creating duplicated objects and apply deserialized data to the found object. We also subscribe to the IBindingList.ListChanged event of the *Features* collection to initialize keys of a newly added object and update the persistent *FeatureList* property whenever the collection is modified.

The **NPFeatureAdapter** class (derived from the common **NonPersistentObjectAdapter** helper class) is used to subscribe to [NonPersistentObjectSpace](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace) events and maintain an object identity map. In the overridden *LoadObjectByKey* method (called when the [ObjectByKeyGetting](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.ObjectByKeyGetting) event is raised), we parse the compound key, locate the owner (*Project*) using *OwnerKey* and search for the desired *Feature* in its *Features* collection using *LocalKey*.

*Files to look at*:
* [Project.cs](./CS/XPO/NonPersistentDemo/NonPersistentDemo.Module/BusinessObjects/CollectionComplete/Project.cs)
* [Feature.cs](./CS/XPO/NonPersistentDemo/NonPersistentDemo.Module/BusinessObjects/CollectionComplete/Feature.cs)

#### Solution B

This solution is demonstrated by the **Department** business object. The non-persistent **Agent** class represents complex collection items. The *Agent* class has a simple key. In *WinApplication* and *WebApplication* descendants we override the **GetObjectSpaceToShowDetailViewFrom** method to reuse the source object space for windows showing *Agent* objects. This approach simplifies the code but changes made to non-persistent objects in separate windows cannot be undone. As a consequence, these windows have no Save and Cancel actions.

The **NPAgentAdapter** class (derived from the common **NonPersistentObjectAdapter** helper class) is used to subscribe to [NonPersistentObjectSpace](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace) events and maintain an object identity map.

*Files to look at*:
* [Department.cs](./CS/XPO/NonPersistentDemo/NonPersistentDemo.Module/BusinessObjects/CollectionInSameSpace/Department.cs)
* [Agent.cs](./CS/XPO/NonPersistentDemo/NonPersistentDemo.Module/BusinessObjects/CollectionInSameSpace/Agent.cs)


### Scenario 3: A nested collection of Non-Persistent objects stored separately

In a persistent business object, we have a string field where we store a sequence of key values. These keys correspond to objects stored elsewhere (in the application model or an external service). We want to show these objects in the UI as a nested list view and allow users to edit the collection by adding and removing items.

This scenario is demonstrated by the **Epoch** business object. The non-persistent **Technology** class represents complex collection items. In this example, we store *Technology* objects in a static dictionary.

In the *Epoch* class, we have a hidden persistent *TechnologyList* string property and a visible non-persistent *Technologies* collection property. We override the *OnLoaded* and *OnSaving* methods to serialize and deserialize the *Technologies* collection. After deserialization we call the [GetObjectByKey](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.BaseObjectSpace.GetObjectByKey--1(System.Object)) method to load related *Technology* objects.

The **NPTechnologyAdapter** class (derived from the common **NonPersistentObjectAdapter** helper class) is used to subscribe to [NonPersistentObjectSpace](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace) events and maintain an object identity map. In the overridden *LoadObjectByKey* method (called when the [ObjectByKeyGetting](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.ObjectByKeyGetting) event is raised), we load *Technology* data from the storage and create object instances. In the overridden *CommitChanges* method (called when the [CustomCommitChanges](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.BaseObjectSpace.CustomCommitChanges) event is raised) we save *Technology* object data to the storage.

*Files to look at*:
* [Epoch.cs](./CS/XPO/NonPersistentDemo/NonPersistentDemo.Module/BusinessObjects/CollectionStoredSeparately/Epoch.cs)
* [Technology.cs](./CS/XPO/NonPersistentDemo/NonPersistentDemo.Module/BusinessObjects/CollectionStoredSeparately/Technology.cs)


## Files to Review

- [Module.cs](./CS/XPO/NonPersistentDemo/NonPersistentDemo.Module/Module.cs)
- [NonPersistentObjectAdapter.cs](./CS/XPO/NonPersistentDemo/NonPersistentDemo.Module/BusinessObjects/NonPersistentObjectAdapter.cs)

## More Examples

- [DevExpress-Examples/XAF_Non-Persistent-Objects-Editing-Demo: How to implement CRUD operations for Non-Persistent Objects stored remotely in eXpressApp Framework](https://github.com/DevExpress-Examples/XAF_Non-Persistent-Objects-Editing-Demo)
- [DevExpress-Examples/XAF_Non-Persistent-Objects-Nested-In-Persistent-Objects-Demo: .NET, Frameworks (XAF & XPO), eXpressApp Framework](https://github.com/DevExpress-Examples/XAF_Non-Persistent-Objects-Nested-In-Persistent-Objects-Demo)
- [DevExpress-Examples/XAF_Non-Persistent-Objects-Reloading-Demo: .NET, Frameworks (XAF & XPO), eXpressApp Framework](https://github.com/DevExpress-Examples/XAF_Non-Persistent-Objects-Reloading-Demo)
- [DevExpress-Examples/XAF_Non-Persistent-Objects-Filtering-Demo: .NET, Frameworks (XAF & XPO), eXpressApp Framework](https://github.com/DevExpress-Examples/XAF_Non-Persistent-Objects-Filtering-Demo)



#### We don't have an EF Core version of this example. If you need this version, please create a ticket in our [Support Center](https://supportcenter.devexpress.com/ticket/list?preset=mytickets) and describe your ultimate goal in detail. We will do our best to assist you.
