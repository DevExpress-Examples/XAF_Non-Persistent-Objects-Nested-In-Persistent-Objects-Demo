<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/278218156/24.2.1%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T919644)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
[![](https://img.shields.io/badge/💬_Leave_Feedback-feecdd?style=flat-square)](#does-this-example-address-your-development-requirementsobjectives)
<!-- default badges end -->

# XAF - How to edit non-persistent objects nested in a persistent object

Often times we are required to store complex data in persistent business objects in compact form (as a string or a byte array), but to show and edit this complex data as objects using standard XAF UI. To address this task in XAF, use [non\-persistent objects](https://docs.devexpress.com/eXpressAppFramework/116516/concepts/business-model-design/non-persistent-objects) nested in persistent business objects as reference or collection properties. This example demonstrates possible implementations for such scenarios.

To create built-in functionality that can applied to a combination of persistent and non-persistent objects, follow the steps below: 

1. In the common **Module**, subscribe to the [XafApplication\.ObjectSpaceCreated](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.XafApplication.ObjectSpaceCreated) event.
2. Call the [CompositeObjectSpace\.PopulateAdditionalObjectSpaces](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.CompositeObjectSpace.PopulateAdditionalObjectSpaces.overloads?p=net6) method.
3. Enable the [AutoCommitAdditionalObjectSpaces](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.CompositeObjectSpace.AutoCommitAdditionalObjectSpaces?p=net6), [AutoRefreshAdditionalObjectSpaces](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.CompositeObjectSpace.AutoRefreshAdditionalObjectSpaces?p=net6), and [AutoSetModifiedOnObjectChange](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.AutoSetModifiedOnObjectChange) options, and set up helpers (adapters) that will handle [NonPersistentObjectSpace](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace) events.

> [!WARNING]
> We created this example for demonstration purposes and it is not intended to address all possible usage scenarios.
> If this example does not have certain functionality or you want to change its behavior, you can extend this example. Note that such an action can be complex and would require good knowledge of XAF: [UI Customization Categories by Skill Level](https://www.devexpress.com/products/net/application_framework/xaf-considerations-for-newcomers.xml#ui-customization-categories) and a possible research of how our components function. Refer to the following help topic for more information: [Debug DevExpress .NET Source Code with PDB Symbols](https://docs.devexpress.com/GeneralInformation/403656/support-debug-troubleshooting/debug-controls-with-debug-symbols).
> We are unable to help with such tasks as custom programming is outside our Support Service purview: [Technical Support Scope](https://www.devexpress.com/products/net/application_framework/xaf-considerations-for-newcomers.xml#support).

## Files to Review

- [Module.cs](./CS/XPO/NonPersistentDemo/NonPersistentDemo.Module/Module.cs)
- [NonPersistentObjectAdapter.cs](./CS/XPO/NonPersistentDemo/NonPersistentDemo.Module/BusinessObjects/NonPersistentObjectAdapter.cs)

## Implementation Details

### Scenario 1: A non-persistent lookup property

If you have a string field in a persistent business object, you can display this field in the UI using a lookup editor so that a user can choose from existing values or add a new value. The list of existing values is created dynamically.

To implement this scenario, do the following:

1. This scenario is demonstrated by the **Product** business object. Add a hidden persistent `GroupName` string property and a visible non-persistent `Group` property. The non-persistent `Group` class defines existing string values and has the `Name` property that is also a key property.
2. In the `Product` class, override the `OnLoaded` method to create a `Group` based on the stored `GroupName` value.
3. Also, override the `OnChanged` method to update the `GroupName` property when the `Group` property is changed.
4. To populate the lookup list view, subscribe to the [NonPersistentObjectSpace\.ObjectsGetting](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.ObjectsGetting) event and collect unique group names from all existing Product objects.

#### Files to Review

* [Product.cs](./CS/XPO/NonPersistentDemo/NonPersistentDemo.Module/BusinessObjects/LookupWithCustomSource/Product.cs)
* [Group.cs](./CS/XPO/NonPersistentDemo/NonPersistentDemo.Module/BusinessObjects/LookupWithCustomSource/Group.cs)


### Scenario 2: A nested collection of non-persistent objects stored in the owner persistent object

In this scenario, a persistent business object includes a string field. This field holds a collection of complex data items serialized to XML. You need to display that collection in the UI as a List View (and not as just a text field that contains XML code). This example creates such a List View that allows users to browse and modify the collection and its individual items.

#### Solution A

1. This solution is demonstrated by the **Project** business object. The non-persistent **Feature** class represents complex collection items. The `Feature` class has a compound key that consists of the `OwnerKey` and `LocalKey` parts. The `OwnerKey` is used to locate the owner object (`Project`). The `LocalKey` is used to identify a `Feature` object within the collection. These keys are not serialized and exist at runtime only.
2. The `Project` class has a hidden persistent `FeatureList` string property and a visible non-persistent `Features` aggregated collection property.
3. Override the `OnLoaded` and `OnSaving` methods to serialize and deserialize the `Features` collection. Note that after deserialization, you should initialize the local key property and the owner key property.
4. Call the [NonPersistentObjectSpace\.GetObject](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.GetObject(System.Object)) method to avoid creation of duplicated objects and to apply deserialized data to the found object.
5. Subscribe to the `IBindingList.ListChanged` event of the `Features` collection to initialize keys of a newly added object and update the persistent `FeatureList` property whenever the collection is modified.
6. The `NPFeatureAdapter` class (derived from the common `NonPersistentObjectAdapter` helper class) is used to subscribe to [NonPersistentObjectSpace](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace) events and maintain an object identity map.
7. In the overridden `LoadObjectByKey` method (called when the [ObjectByKeyGetting](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.ObjectByKeyGetting) event is raised), parse the compound key, locate the owner (`Project`) using `OwnerKey`, and search for the desired `Feature` in its `Features` collection using `LocalKey`.

##### Files to Review

* [Project.cs](./CS/XPO/NonPersistentDemo/NonPersistentDemo.Module/BusinessObjects/CollectionComplete/Project.cs)
* [Feature.cs](./CS/XPO/NonPersistentDemo/NonPersistentDemo.Module/BusinessObjects/CollectionComplete/Feature.cs)

#### Solution B

1. This solution is demonstrated by the **Department** business object. The non-persistent **Agent** class stores complex collection items and has a simple key.
2. In `WinApplication` and `WebApplication`, descendants override the `GetObjectSpaceToShowDetailViewFrom` method to reuse the source object space for windows that display `Agent` objects. This approach simplifies code, but changes made to non-persistent objects in separate windows cannot be undone. As a result, these windows do not have **Save** and **Cancel** actions.
3. The `NPAgentAdapter` class (derived from the common `NonPersistentObjectAdapter` helper class) is used to subscribe to [NonPersistentObjectSpace](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace) events and maintain an object identity map.

##### Files to Review

* [Department.cs](./CS/XPO/NonPersistentDemo/NonPersistentDemo.Module/BusinessObjects/CollectionInSameSpace/Department.cs)
* [Agent.cs](./CS/XPO/NonPersistentDemo/NonPersistentDemo.Module/BusinessObjects/CollectionInSameSpace/Agent.cs)


### Scenario 3: A nested collection of non-persistent objects stored separately

In a persistent business object, you have a string field where we store a sequence of key values. These keys correspond to objects stored elsewhere (in the application model or in an external service). You need to show these objects in the UI as a nested list view and allow users to edit the collection by adding and removing items.

1. This scenario is demonstrated by the **Epoch** business object. The non-persistent **Technology** class represents complex collection items. In this example, a static dictionary stores `Technology` objects.
2. The `Epoch` class has a hidden persistent `TechnologyList` string property and a visible non-persistent `Technologies` collection property.
3. Override the `OnLoaded` and `OnSaving` methods to serialize and deserialize the `Technologies` collection.
4. After deserialization, call the [GetObjectByKey](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.BaseObjectSpace.GetObjectByKey--1(System.Object)) method to load related `Technology` objects.
5. The `NPTechnologyAdapter` class (derived from the common `NonPersistentObjectAdapter` helper class) is used to subscribe to [NonPersistentObjectSpace](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace) events and maintain an object identity map.
6. In the overridden `LoadObjectByKey` method (called when the [ObjectByKeyGetting](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.NonPersistentObjectSpace.ObjectByKeyGetting) event is raised), load `Technology` data from storage and create object instances.
7. In the overridden `CommitChanges` method (called when the [CustomCommitChanges](https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.BaseObjectSpace.CustomCommitChanges) event is raised), save `Technology` object data to storage.

##### Files to Review 

* [Epoch.cs](./CS/XPO/NonPersistentDemo/NonPersistentDemo.Module/BusinessObjects/CollectionStoredSeparately/Epoch.cs)
* [Technology.cs](./CS/XPO/NonPersistentDemo/NonPersistentDemo.Module/BusinessObjects/CollectionStoredSeparately/Technology.cs)

## Documentation

- [Non-Persistent Objects](https://docs.devexpress.com/eXpressAppFramework/116516/business-model-design-orm/non-persistent-objects)


## More Examples

- [How to implement CRUD operations for Non-Persistent Objects stored remotely in eXpressApp Framework](https://github.com/DevExpress-Examples/XAF_Non-Persistent-Objects-Editing-Demo)
- [How to edit Non-Persistent Objects nested in a Persistent Object](https://github.com/DevExpress-Examples/XAF_Non-Persistent-Objects-Nested-In-Persistent-Objects-Demo)
- [How to: Display a List of Non-Persistent Objects](https://github.com/DevExpress-Examples/XAF_how-to-display-a-list-of-non-persistent-objects-e980)
- [How to filter and sort Non-Persistent Objects](https://github.com/DevExpress-Examples/XAF_Non-Persistent-Objects-Filtering-Demo)
- [How to refresh Non-Persistent Objects and reload nested Persistent Objects](https://github.com/DevExpress-Examples/XAF_Non-Persistent-Objects-Reloading-Demo)
- [How to edit a collection of Persistent Objects linked to a Non-Persistent Object](https://github.com/DevExpress-Examples/XAF_Non-Persistent-Objects-Edit-Linked-Persistent-Objects-Demo)


#### We don't have an EF Core version of this example. If you need this version, please create a ticket in our [Support Center](https://supportcenter.devexpress.com/ticket/list?preset=mytickets) and describe your ultimate goal in detail. We will do our best to assist you.
<!-- feedback -->
## Does this example address your development requirements/objectives?

[<img src="https://www.devexpress.com/support/examples/i/yes-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=XAF_Non-Persistent-Objects-Nested-In-Persistent-Objects-Demo&~~~was_helpful=yes) [<img src="https://www.devexpress.com/support/examples/i/no-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=XAF_Non-Persistent-Objects-Nested-In-Persistent-Objects-Demo&~~~was_helpful=no)

(you will be redirected to DevExpress.com to submit your response)
<!-- feedback end -->
