[assembly: System.CLSCompliantAttribute(true)]
[assembly: System.Runtime.CompilerServices.InternalsVisibleToAttribute(@"Castle.Core.Tests, PublicKey=002400000480000094000000060200000024000052534131000400000100010077f5e87030dadccce6902c6adab7a987bd69cb5819991531f560785eacfc89b6fcddf6bb2a00743a7194e454c0273447fc6eec36474ba8e5a3823147d214298e4f9a631b1afee1a51ffeae4672d498f14b000e3d321453cdd8ac064de7e1cf4d222b7e81f54d4fd46725370d702a05b48738cc29d09228f1aa722ae1a9ca02fb")]
[assembly: System.Runtime.InteropServices.ComVisibleAttribute(false)]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETStandard,Version=v1.5", FrameworkDisplayName="")]
namespace Castle.Components.DictionaryAdapter
{
    public abstract class AbstractDictionaryAdapter : System.Collections.ICollection, System.Collections.IDictionary, System.Collections.IEnumerable
    {
        protected AbstractDictionaryAdapter() { }
        public int Count { get; }
        public bool IsFixedSize { get; }
        public abstract bool IsReadOnly { get; }
        public virtual bool IsSynchronized { get; }
        public abstract object this[object key] { get; set; }
        public System.Collections.ICollection Keys { get; }
        public virtual object SyncRoot { get; }
        public System.Collections.ICollection Values { get; }
        public void Add(object key, object value) { }
        public void Clear() { }
        public abstract bool Contains(object key);
        public void CopyTo(System.Array array, int index) { }
        public System.Collections.IDictionaryEnumerator GetEnumerator() { }
        public void Remove(object key) { }
    }
    public abstract class AbstractDictionaryAdapterVisitor : Castle.Components.DictionaryAdapter.IDictionaryAdapterVisitor
    {
        protected AbstractDictionaryAdapterVisitor() { }
        protected AbstractDictionaryAdapterVisitor(Castle.Components.DictionaryAdapter.AbstractDictionaryAdapterVisitor parent) { }
        protected bool Cancelled { get; set; }
        protected virtual void VisitCollection(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, Castle.Components.DictionaryAdapter.PropertyDescriptor property, System.Type collectionItemType, object state) { }
        public virtual bool VisitDictionaryAdapter(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, object state) { }
        public virtual bool VisitDictionaryAdapter(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, System.Func<Castle.Components.DictionaryAdapter.PropertyDescriptor, bool> selector, object state) { }
        protected virtual void VisitInterface(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, Castle.Components.DictionaryAdapter.PropertyDescriptor property, object state) { }
        protected virtual void VisitProperty(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, Castle.Components.DictionaryAdapter.PropertyDescriptor property, object state) { }
    }
    public class CascadingDictionaryAdapter : Castle.Components.DictionaryAdapter.AbstractDictionaryAdapter
    {
        public CascadingDictionaryAdapter(System.Collections.IDictionary primary, System.Collections.IDictionary secondary) { }
        public override bool IsReadOnly { get; }
        public override object this[object key] { get; set; }
        public System.Collections.IDictionary Primary { get; }
        public System.Collections.IDictionary Secondary { get; }
        public override bool Contains(object key) { }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Property | System.AttributeTargets.All, AllowMultiple=false, Inherited=true)]
    public class ComponentAttribute : Castle.Components.DictionaryAdapter.DictionaryBehaviorAttribute, Castle.Components.DictionaryAdapter.IDictionaryBehavior, Castle.Components.DictionaryAdapter.IDictionaryKeyBuilder, Castle.Components.DictionaryAdapter.IDictionaryPropertyGetter, Castle.Components.DictionaryAdapter.IDictionaryPropertySetter
    {
        public ComponentAttribute() { }
        public bool NoPrefix { get; set; }
        public string Prefix { get; set; }
        public bool SetPropertyValue(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, string key, ref object value, Castle.Components.DictionaryAdapter.PropertyDescriptor property) { }
    }
    public class DefaultPropertyGetter : Castle.Components.DictionaryAdapter.IDictionaryBehavior, Castle.Components.DictionaryAdapter.IDictionaryPropertyGetter
    {
        public DefaultPropertyGetter(System.ComponentModel.TypeConverter converter) { }
        public int ExecutionOrder { get; }
        public Castle.Components.DictionaryAdapter.IDictionaryBehavior Copy() { }
        public object GetPropertyValue(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, string key, object storedValue, Castle.Components.DictionaryAdapter.PropertyDescriptor property, bool ifExists) { }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Class | System.AttributeTargets.All, AllowMultiple=false, Inherited=false)]
    public class DictionaryAdapterAttribute : System.Attribute
    {
        public DictionaryAdapterAttribute(System.Type interfaceType) { }
        public System.Type InterfaceType { get; }
    }
    public abstract class DictionaryAdapterBase : Castle.Components.DictionaryAdapter.IDictionaryAdapter, Castle.Components.DictionaryAdapter.IDictionaryCreate, Castle.Components.DictionaryAdapter.IDictionaryEdit, Castle.Components.DictionaryAdapter.IDictionaryNotify, Castle.Components.DictionaryAdapter.IDictionaryValidate, System.ComponentModel.IChangeTracking, System.ComponentModel.IEditableObject, System.ComponentModel.INotifyPropertyChanged, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.IRevertibleChangeTracking
    {
        public DictionaryAdapterBase(Castle.Components.DictionaryAdapter.DictionaryAdapterInstance instance) { }
        public bool CanEdit { get; set; }
        public bool CanNotify { get; set; }
        public bool CanValidate { get; set; }
        public bool IsChanged { get; }
        public bool IsEditing { get; }
        public bool IsValid { get; }
        public abstract Castle.Components.DictionaryAdapter.DictionaryAdapterMeta Meta { get; }
        public bool ShouldNotify { get; }
        public bool SupportsMultiLevelEdit { get; set; }
        public Castle.Components.DictionaryAdapter.DictionaryAdapterInstance This { get; }
        public System.Collections.Generic.IEnumerable<Castle.Components.DictionaryAdapter.IDictionaryValidator> Validators { get; }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public event System.ComponentModel.PropertyChangingEventHandler PropertyChanging;
        public void AcceptChanges() { }
        protected void AddEditDependency(System.ComponentModel.IEditableObject editDependency) { }
        public void AddValidator(Castle.Components.DictionaryAdapter.IDictionaryValidator validator) { }
        public void BeginEdit() { }
        public void CancelEdit() { }
        protected bool ClearEditProperty(Castle.Components.DictionaryAdapter.PropertyDescriptor property, string key) { }
        public void ClearProperty(Castle.Components.DictionaryAdapter.PropertyDescriptor property, string key) { }
        public T Coerce<T>()
            where T :  class { }
        public object Coerce(System.Type type) { }
        public void CopyTo(Castle.Components.DictionaryAdapter.IDictionaryAdapter other) { }
        public void CopyTo(Castle.Components.DictionaryAdapter.IDictionaryAdapter other, System.Func<Castle.Components.DictionaryAdapter.PropertyDescriptor, bool> selector) { }
        public T Create<T>() { }
        public object Create(System.Type type) { }
        public T Create<T>(System.Collections.IDictionary dictionary) { }
        public object Create(System.Type type, System.Collections.IDictionary dictionary) { }
        public T Create<T>(System.Action<T> init) { }
        public T Create<T>(System.Collections.IDictionary dictionary, System.Action<T> init) { }
        protected bool EditProperty(Castle.Components.DictionaryAdapter.PropertyDescriptor property, string key, object propertyValue) { }
        public void EndEdit() { }
        public override bool Equals(object obj) { }
        protected bool GetEditedProperty(string propertyName, out object propertyValue) { }
        public override int GetHashCode() { }
        public string GetKey(string propertyName) { }
        public virtual object GetProperty(string propertyName, bool ifExists) { }
        public T GetPropertyOfType<T>(string propertyName) { }
        protected void Initialize() { }
        protected void Invalidate() { }
        protected void NotifyPropertyChanged(Castle.Components.DictionaryAdapter.PropertyDescriptor property, object oldValue, object newValue) { }
        protected void NotifyPropertyChanged(string propertyName) { }
        protected bool NotifyPropertyChanging(Castle.Components.DictionaryAdapter.PropertyDescriptor property, object oldValue, object newValue) { }
        public object ReadProperty(string key) { }
        public void RejectChanges() { }
        public void ResumeEditing() { }
        public void ResumeNotifications() { }
        public virtual bool SetProperty(string propertyName, ref object value) { }
        public bool ShouldClearProperty(Castle.Components.DictionaryAdapter.PropertyDescriptor property, object value) { }
        public void StoreProperty(Castle.Components.DictionaryAdapter.PropertyDescriptor property, string key, object value) { }
        public void SuppressEditing() { }
        public System.IDisposable SuppressEditingBlock() { }
        public void SuppressNotifications() { }
        public System.IDisposable SuppressNotificationsBlock() { }
        protected Castle.Components.DictionaryAdapter.DictionaryAdapterBase.TrackPropertyChangeScope TrackPropertyChange(Castle.Components.DictionaryAdapter.PropertyDescriptor property, object oldValue, object newValue) { }
        protected Castle.Components.DictionaryAdapter.DictionaryAdapterBase.TrackPropertyChangeScope TrackReadonlyPropertyChanges() { }
        public Castle.Components.DictionaryAdapter.DictionaryValidateGroup ValidateGroups(params object[] groups) { }
        public class TrackPropertyChangeScope : System.IDisposable
        {
            public TrackPropertyChangeScope(Castle.Components.DictionaryAdapter.DictionaryAdapterBase adapter) { }
            public TrackPropertyChangeScope(Castle.Components.DictionaryAdapter.DictionaryAdapterBase adapter, Castle.Components.DictionaryAdapter.PropertyDescriptor property, object existingValue) { }
            public void Dispose() { }
            public bool Notify() { }
        }
    }
    public class static DictionaryAdapterExtensions
    {
        public static Castle.Components.DictionaryAdapter.IVirtual AsVirtual(this Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter) { }
    }
    public class DictionaryAdapterFactory : Castle.Components.DictionaryAdapter.IDictionaryAdapterFactory
    {
        public DictionaryAdapterFactory() { }
        public T GetAdapter<T>(System.Collections.IDictionary dictionary) { }
        public object GetAdapter(System.Type type, System.Collections.IDictionary dictionary) { }
        public object GetAdapter(System.Type type, System.Collections.IDictionary dictionary, Castle.Components.DictionaryAdapter.PropertyDescriptor descriptor) { }
        public T GetAdapter<T, R>(System.Collections.Generic.IDictionary<string, R> dictionary) { }
        public object GetAdapter<R>(System.Type type, System.Collections.Generic.IDictionary<string, R> dictionary) { }
        public T GetAdapter<T>(System.Collections.Specialized.NameValueCollection nameValues) { }
        public object GetAdapter(System.Type type, System.Collections.Specialized.NameValueCollection nameValues) { }
        public Castle.Components.DictionaryAdapter.DictionaryAdapterMeta GetAdapterMeta(System.Type type) { }
        public Castle.Components.DictionaryAdapter.DictionaryAdapterMeta GetAdapterMeta(System.Type type, Castle.Components.DictionaryAdapter.PropertyDescriptor descriptor) { }
        public Castle.Components.DictionaryAdapter.DictionaryAdapterMeta GetAdapterMeta(System.Type type, Castle.Components.DictionaryAdapter.DictionaryAdapterMeta other) { }
    }
    public class DictionaryAdapterInstance
    {
        public DictionaryAdapterInstance(System.Collections.IDictionary dictionary, Castle.Components.DictionaryAdapter.DictionaryAdapterMeta meta, Castle.Components.DictionaryAdapter.PropertyDescriptor descriptor, Castle.Components.DictionaryAdapter.IDictionaryAdapterFactory factory) { }
        public Castle.Components.DictionaryAdapter.IDictionaryCoerceStrategy CoerceStrategy { get; set; }
        public System.Collections.Generic.IEnumerable<Castle.Components.DictionaryAdapter.IDictionaryCopyStrategy> CopyStrategies { get; }
        public Castle.Components.DictionaryAdapter.IDictionaryCreateStrategy CreateStrategy { get; set; }
        public Castle.Components.DictionaryAdapter.PropertyDescriptor Descriptor { get; }
        public System.Collections.IDictionary Dictionary { get; }
        public Castle.Components.DictionaryAdapter.IDictionaryEqualityHashCodeStrategy EqualityHashCodeStrategy { get; set; }
        public System.Collections.IDictionary ExtendedProperties { get; }
        public Castle.Components.DictionaryAdapter.IDictionaryAdapterFactory Factory { get; }
        public Castle.Components.DictionaryAdapter.IDictionaryInitializer[] Initializers { get; }
        public System.Collections.Generic.IDictionary<string, Castle.Components.DictionaryAdapter.PropertyDescriptor> Properties { get; }
        public void AddCopyStrategy(Castle.Components.DictionaryAdapter.IDictionaryCopyStrategy copyStrategy) { }
    }
    [System.Diagnostics.DebuggerDisplayAttribute("Type: {Type.FullName,nq}")]
    public class DictionaryAdapterMeta
    {
        public DictionaryAdapterMeta(System.Type type, System.Type implementation, object[] behaviors, Castle.Components.DictionaryAdapter.IDictionaryMetaInitializer[] metaInitializers, Castle.Components.DictionaryAdapter.IDictionaryInitializer[] initializers, System.Collections.Generic.IDictionary<string, Castle.Components.DictionaryAdapter.PropertyDescriptor> properties, Castle.Components.DictionaryAdapter.IDictionaryAdapterFactory factory, System.Func<Castle.Components.DictionaryAdapter.DictionaryAdapterInstance, Castle.Components.DictionaryAdapter.IDictionaryAdapter> creator) { }
        public object[] Behaviors { get; }
        public System.Collections.IDictionary ExtendedProperties { get; }
        public Castle.Components.DictionaryAdapter.IDictionaryAdapterFactory Factory { get; }
        public System.Type Implementation { get; }
        public Castle.Components.DictionaryAdapter.IDictionaryInitializer[] Initializers { get; }
        public Castle.Components.DictionaryAdapter.IDictionaryMetaInitializer[] MetaInitializers { get; }
        public System.Collections.Generic.IDictionary<string, Castle.Components.DictionaryAdapter.PropertyDescriptor> Properties { get; }
        public System.Type Type { get; }
        public Castle.Components.DictionaryAdapter.PropertyDescriptor CreateDescriptor() { }
        public object CreateInstance(System.Collections.IDictionary dictionary, Castle.Components.DictionaryAdapter.PropertyDescriptor descriptor) { }
        public Castle.Components.DictionaryAdapter.DictionaryAdapterMeta GetAdapterMeta(System.Type type) { }
    }
    public abstract class DictionaryBehaviorAttribute : System.Attribute, Castle.Components.DictionaryAdapter.IDictionaryBehavior
    {
        public const int DefaultExecutionOrder = 1073741823;
        public const int FirstExecutionOrder = 0;
        public const int LastExecutionOrder = 2147483647;
        public DictionaryBehaviorAttribute() { }
        public int ExecutionOrder { get; set; }
        public virtual Castle.Components.DictionaryAdapter.IDictionaryBehavior Copy() { }
    }
    public class DictionaryValidateGroup : Castle.Components.DictionaryAdapter.IDictionaryValidate, System.ComponentModel.INotifyPropertyChanged, System.IDisposable
    {
        public DictionaryValidateGroup(object[] groups, Castle.Components.DictionaryAdapter.IDictionaryAdapter adapter) { }
        public bool CanValidate { get; set; }
        public bool IsValid { get; }
        public System.Collections.Generic.IEnumerable<Castle.Components.DictionaryAdapter.IDictionaryValidator> Validators { get; }
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public void AddValidator(Castle.Components.DictionaryAdapter.IDictionaryValidator validator) { }
        public void Dispose() { }
        public Castle.Components.DictionaryAdapter.DictionaryValidateGroup ValidateGroups(params object[] groups) { }
    }
    public class DynamicDictionary : System.Dynamic.DynamicObject
    {
        public DynamicDictionary(System.Collections.IDictionary dictionary) { }
        public override System.Collections.Generic.IEnumerable<string> GetDynamicMemberNames() { }
        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result) { }
        public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value) { }
    }
    public class DynamicValueDelegate<T> : Castle.Components.DictionaryAdapter.DynamicValue<T>
    {
        public DynamicValueDelegate(System.Func<T> dynamicDelegate) { }
        public override T Value { get; }
    }
    public abstract class DynamicValue<T> : Castle.Components.DictionaryAdapter.IDynamicValue, Castle.Components.DictionaryAdapter.IDynamicValue<T>
    {
        protected DynamicValue() { }
        public abstract T Value { get; }
        public override string ToString() { }
    }
    public class EditableList : Castle.Components.DictionaryAdapter.EditableList<object>, System.Collections.ICollection, System.Collections.IEnumerable, System.Collections.IList
    {
        public EditableList() { }
        public EditableList(System.Collections.Generic.IEnumerable<object> collection) { }
    }
    public class EditableList<T> : System.Collections.Generic.List<T>, System.ComponentModel.IChangeTracking, System.ComponentModel.IEditableObject, System.ComponentModel.IRevertibleChangeTracking
    {
        public EditableList() { }
        public EditableList(System.Collections.Generic.IEnumerable<T> collection) { }
        public bool IsChanged { get; }
        public void AcceptChanges() { }
        public void BeginEdit() { }
        public void CancelEdit() { }
        public void EndEdit() { }
        public void RejectChanges() { }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Property | System.AttributeTargets.Interface | System.AttributeTargets.All, AllowMultiple=false)]
    public class FetchAttribute : System.Attribute
    {
        public FetchAttribute() { }
        public FetchAttribute(bool fetch) { }
        public bool Fetch { get; }
    }
    public class static GenericDictionaryAdapter
    {
        public static Castle.Components.DictionaryAdapter.GenericDictionaryAdapter<TValue> ForDictionaryAdapter<TValue>(this System.Collections.Generic.IDictionary<string, TValue> dictionary) { }
    }
    public class GenericDictionaryAdapter<TValue> : Castle.Components.DictionaryAdapter.AbstractDictionaryAdapter
    {
        public GenericDictionaryAdapter(System.Collections.Generic.IDictionary<string, TValue> dictionary) { }
        public override bool IsReadOnly { get; }
        public override object this[object key] { get; set; }
        public override bool Contains(object key) { }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Property | System.AttributeTargets.All, AllowMultiple=true)]
    public class GroupAttribute : System.Attribute
    {
        public GroupAttribute(object group) { }
        public GroupAttribute(params object[] group) { }
        public object[] Group { get; }
    }
    public interface IBindingList<T> : System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>, System.Collections.Generic.IList<T>, System.Collections.IEnumerable
    {
        bool AllowEdit { get; }
        bool AllowNew { get; }
        bool AllowRemove { get; }
        bool IsSorted { get; }
        System.ComponentModel.PropertyDescriptor SortProperty { get; }
        bool SupportsChangeNotification { get; }
        bool SupportsSearching { get; }
        bool SupportsSorting { get; }
        void AddIndex(System.ComponentModel.PropertyDescriptor property);
        T AddNew();
        int Find(System.ComponentModel.PropertyDescriptor property, object key);
        void RemoveIndex(System.ComponentModel.PropertyDescriptor property);
        void RemoveSort();
    }
    public interface ICollectionAdapterObserver<T>
    {
        void OnInserted(T newValue, int index);
        bool OnInserting(T newValue);
        void OnRemoved(T oldValue, int index);
        void OnRemoving(T oldValue);
        void OnReplaced(T oldValue, T newValue, int index);
        bool OnReplacing(T oldValue, T newValue);
    }
    public interface ICollectionAdapter<T>
    {
        System.Collections.Generic.IEqualityComparer<T> Comparer { get; }
        int Count { get; }
        bool HasSnapshot { get; }
        T this[int index] { get; set; }
        int SnapshotCount { get; }
        bool Add(T value);
        T AddNew();
        void Clear();
        void ClearReferences();
        void DropSnapshot();
        T GetCurrentItem(int index);
        T GetSnapshotItem(int index);
        void Initialize(Castle.Components.DictionaryAdapter.ICollectionAdapterObserver<T> advisor);
        bool Insert(int index, T value);
        void LoadSnapshot();
        void Remove(int index);
        void SaveSnapshot();
    }
    public interface ICollectionProjection : System.Collections.ICollection, System.Collections.IEnumerable
    {
        void Clear();
        void ClearReferences();
        void Replace(System.Collections.IEnumerable source);
    }
    public interface ICondition
    {
        bool SatisfiedBy(object value);
    }
    public interface IDictionaryAdapter : Castle.Components.DictionaryAdapter.IDictionaryCreate, Castle.Components.DictionaryAdapter.IDictionaryEdit, Castle.Components.DictionaryAdapter.IDictionaryNotify, Castle.Components.DictionaryAdapter.IDictionaryValidate, System.ComponentModel.IChangeTracking, System.ComponentModel.IEditableObject, System.ComponentModel.INotifyPropertyChanged, System.ComponentModel.INotifyPropertyChanging, System.ComponentModel.IRevertibleChangeTracking
    {
        Castle.Components.DictionaryAdapter.DictionaryAdapterMeta Meta { get; }
        Castle.Components.DictionaryAdapter.DictionaryAdapterInstance This { get; }
        void ClearProperty(Castle.Components.DictionaryAdapter.PropertyDescriptor property, string key);
        T Coerce<T>()
            where T :  class;
        object Coerce(System.Type type);
        void CopyTo(Castle.Components.DictionaryAdapter.IDictionaryAdapter other);
        void CopyTo(Castle.Components.DictionaryAdapter.IDictionaryAdapter other, System.Func<Castle.Components.DictionaryAdapter.PropertyDescriptor, bool> selector);
        string GetKey(string propertyName);
        object GetProperty(string propertyName, bool ifExists);
        T GetPropertyOfType<T>(string propertyName);
        object ReadProperty(string key);
        bool SetProperty(string propertyName, ref object value);
        bool ShouldClearProperty(Castle.Components.DictionaryAdapter.PropertyDescriptor property, object value);
        void StoreProperty(Castle.Components.DictionaryAdapter.PropertyDescriptor property, string key, object value);
    }
    public interface IDictionaryAdapterFactory
    {
        T GetAdapter<T>(System.Collections.IDictionary dictionary);
        object GetAdapter(System.Type type, System.Collections.IDictionary dictionary);
        object GetAdapter(System.Type type, System.Collections.IDictionary dictionary, Castle.Components.DictionaryAdapter.PropertyDescriptor descriptor);
        T GetAdapter<T>(System.Collections.Specialized.NameValueCollection nameValues);
        object GetAdapter(System.Type type, System.Collections.Specialized.NameValueCollection nameValues);
        Castle.Components.DictionaryAdapter.DictionaryAdapterMeta GetAdapterMeta(System.Type type);
        Castle.Components.DictionaryAdapter.DictionaryAdapterMeta GetAdapterMeta(System.Type type, Castle.Components.DictionaryAdapter.PropertyDescriptor descriptor);
        Castle.Components.DictionaryAdapter.DictionaryAdapterMeta GetAdapterMeta(System.Type type, Castle.Components.DictionaryAdapter.DictionaryAdapterMeta other);
    }
    public interface IDictionaryAdapterVisitor
    {
        void VisitCollection(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, Castle.Components.DictionaryAdapter.PropertyDescriptor property, System.Type collectionItemType, object state);
        bool VisitDictionaryAdapter(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, object state);
        bool VisitDictionaryAdapter(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, System.Func<Castle.Components.DictionaryAdapter.PropertyDescriptor, bool> selector, object state);
        void VisitInterface(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, Castle.Components.DictionaryAdapter.PropertyDescriptor property, object state);
        void VisitProperty(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, Castle.Components.DictionaryAdapter.PropertyDescriptor property, object state);
    }
    public interface IDictionaryBehavior
    {
        int ExecutionOrder { get; }
        Castle.Components.DictionaryAdapter.IDictionaryBehavior Copy();
    }
    public interface IDictionaryBehaviorBuilder
    {
        object[] BuildBehaviors();
    }
    public interface IDictionaryCoerceStrategy
    {
        object Coerce(Castle.Components.DictionaryAdapter.IDictionaryAdapter adapter, System.Type type);
    }
    public interface IDictionaryCopyStrategy
    {
        bool Copy(Castle.Components.DictionaryAdapter.IDictionaryAdapter source, Castle.Components.DictionaryAdapter.IDictionaryAdapter target, ref System.Func<Castle.Components.DictionaryAdapter.PropertyDescriptor, bool> selector);
    }
    public interface IDictionaryCreate
    {
        T Create<T>();
        object Create(System.Type type);
        T Create<T>(System.Collections.IDictionary dictionary);
        object Create(System.Type type, System.Collections.IDictionary dictionary);
        T Create<T>(System.Action<T> init);
        T Create<T>(System.Collections.IDictionary dictionary, System.Action<T> init);
    }
    public interface IDictionaryCreateStrategy
    {
        object Create(Castle.Components.DictionaryAdapter.IDictionaryAdapter adapter, System.Type type, System.Collections.IDictionary dictionary);
    }
    public interface IDictionaryEdit : System.ComponentModel.IChangeTracking, System.ComponentModel.IEditableObject, System.ComponentModel.IRevertibleChangeTracking
    {
        bool CanEdit { get; }
        bool IsEditing { get; }
        bool SupportsMultiLevelEdit { get; set; }
        void ResumeEditing();
        void SuppressEditing();
        System.IDisposable SuppressEditingBlock();
    }
    public interface IDictionaryEqualityHashCodeStrategy
    {
        bool Equals(Castle.Components.DictionaryAdapter.IDictionaryAdapter adapter1, Castle.Components.DictionaryAdapter.IDictionaryAdapter adapter2);
        bool GetHashCode(Castle.Components.DictionaryAdapter.IDictionaryAdapter adapter, out int hashCode);
    }
    public interface IDictionaryInitializer : Castle.Components.DictionaryAdapter.IDictionaryBehavior
    {
        void Initialize(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, object[] behaviors);
    }
    public interface IDictionaryKeyBuilder : Castle.Components.DictionaryAdapter.IDictionaryBehavior
    {
        string GetKey(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, string key, Castle.Components.DictionaryAdapter.PropertyDescriptor property);
    }
    public interface IDictionaryMetaInitializer : Castle.Components.DictionaryAdapter.IDictionaryBehavior
    {
        void Initialize(Castle.Components.DictionaryAdapter.IDictionaryAdapterFactory factory, Castle.Components.DictionaryAdapter.DictionaryAdapterMeta dictionaryMeta);
        bool ShouldHaveBehavior(object behavior);
    }
    public interface IDictionaryNotify : System.ComponentModel.INotifyPropertyChanged, System.ComponentModel.INotifyPropertyChanging
    {
        bool CanNotify { get; }
        bool ShouldNotify { get; }
        void ResumeNotifications();
        void SuppressNotifications();
        System.IDisposable SuppressNotificationsBlock();
    }
    public interface IDictionaryPropertyGetter : Castle.Components.DictionaryAdapter.IDictionaryBehavior
    {
        object GetPropertyValue(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, string key, object storedValue, Castle.Components.DictionaryAdapter.PropertyDescriptor property, bool ifExists);
    }
    public interface IDictionaryPropertySetter : Castle.Components.DictionaryAdapter.IDictionaryBehavior
    {
        bool SetPropertyValue(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, string key, ref object value, Castle.Components.DictionaryAdapter.PropertyDescriptor property);
    }
    public interface IDictionaryReferenceManager
    {
        void AddReference(object keyObject, object relatedObject, bool isInGraph);
        bool IsReferenceProperty(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, string propertyName);
        bool TryGetReference(object keyObject, out object inGraphObject);
    }
    public interface IDictionaryValidate
    {
        bool CanValidate { get; set; }
        bool IsValid { get; }
        System.Collections.Generic.IEnumerable<Castle.Components.DictionaryAdapter.IDictionaryValidator> Validators { get; }
        void AddValidator(Castle.Components.DictionaryAdapter.IDictionaryValidator validator);
        Castle.Components.DictionaryAdapter.DictionaryValidateGroup ValidateGroups(params object[] groups);
    }
    public interface IDictionaryValidator
    {
        void Invalidate(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter);
        bool IsValid(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter);
        string Validate(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter);
        string Validate(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, Castle.Components.DictionaryAdapter.PropertyDescriptor property);
    }
    public interface IDynamicValue
    {
        object GetValue();
    }
    public interface IDynamicValue<T> : Castle.Components.DictionaryAdapter.IDynamicValue
    {
        T Value { get; }
    }
    public interface IPropertyDescriptorInitializer : Castle.Components.DictionaryAdapter.IDictionaryBehavior
    {
        void Initialize(Castle.Components.DictionaryAdapter.PropertyDescriptor propertyDescriptor, object[] behaviors);
    }
    public interface IValueInitializer
    {
        void Initialize(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, object value);
    }
    public interface IVirtual
    {
        bool IsReal { get; }
        public event System.EventHandler Realized;
        void Realize();
    }
    public interface IVirtualSite<T>
    {
        void OnRealizing(T node);
    }
    public interface IVirtualTarget<TNode, TMember>
    {
        void OnRealizing(TNode node, TMember member);
    }
    public interface IVirtual<T> : Castle.Components.DictionaryAdapter.IVirtual
    {
        void AddSite(Castle.Components.DictionaryAdapter.IVirtualSite<T> site);
        T Realize();
        void RemoveSite(Castle.Components.DictionaryAdapter.IVirtualSite<T> site);
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Property | System.AttributeTargets.All, AllowMultiple=false)]
    public class IfExistsAttribute : System.Attribute
    {
        public IfExistsAttribute() { }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Property | System.AttributeTargets.All, AllowMultiple=false, Inherited=true)]
    public class KeyAttribute : Castle.Components.DictionaryAdapter.DictionaryBehaviorAttribute, Castle.Components.DictionaryAdapter.IDictionaryBehavior, Castle.Components.DictionaryAdapter.IDictionaryKeyBuilder
    {
        public KeyAttribute(string key) { }
        public KeyAttribute(string[] keys) { }
        public string Key { get; }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Interface | System.AttributeTargets.All, AllowMultiple=false, Inherited=false)]
    public class KeyPrefixAttribute : Castle.Components.DictionaryAdapter.DictionaryBehaviorAttribute, Castle.Components.DictionaryAdapter.IDictionaryBehavior, Castle.Components.DictionaryAdapter.IDictionaryKeyBuilder
    {
        public KeyPrefixAttribute() { }
        public KeyPrefixAttribute(string keyPrefix) { }
        public string KeyPrefix { get; set; }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Property | System.AttributeTargets.Interface | System.AttributeTargets.All, AllowMultiple=true, Inherited=true)]
    public class KeySubstitutionAttribute : Castle.Components.DictionaryAdapter.DictionaryBehaviorAttribute, Castle.Components.DictionaryAdapter.IDictionaryBehavior, Castle.Components.DictionaryAdapter.IDictionaryKeyBuilder
    {
        public KeySubstitutionAttribute(string oldValue, string newValue) { }
    }
    [System.Diagnostics.DebuggerDisplayAttribute("Count = {Count}, Adapter = {Adapter}")]
    [System.Diagnostics.DebuggerTypeProxyAttribute(typeof(Castle.Components.DictionaryAdapter.ListProjectionDebugView<T>))]
    public class ListProjection<T> : Castle.Components.DictionaryAdapter.IBindingList<T>, Castle.Components.DictionaryAdapter.ICollectionAdapterObserver<T>, Castle.Components.DictionaryAdapter.ICollectionProjection, System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<T>, System.Collections.Generic.IList<T>, System.Collections.ICollection, System.Collections.IEnumerable, System.Collections.IList, System.ComponentModel.IChangeTracking, System.ComponentModel.IEditableObject, System.ComponentModel.IRevertibleChangeTracking
    {
        public ListProjection(Castle.Components.DictionaryAdapter.ICollectionAdapter<T> adapter) { }
        public Castle.Components.DictionaryAdapter.ICollectionAdapter<T> Adapter { get; }
        public System.Collections.Generic.IEqualityComparer<T> Comparer { get; }
        public int Count { get; }
        public bool EventsEnabled { get; }
        public bool IsChanged { get; }
        public T this[int index] { get; set; }
        public void AcceptChanges() { }
        public virtual bool Add(T item) { }
        public virtual T AddNew() { }
        public void BeginEdit() { }
        public void CancelEdit() { }
        public virtual void CancelNew(int index) { }
        public virtual void Clear() { }
        public virtual bool Contains(T item) { }
        public void CopyTo(T[] array, int index) { }
        public void EndEdit() { }
        public virtual void EndNew(int index) { }
        public System.Collections.Generic.IEnumerator<T> GetEnumerator() { }
        public int IndexOf(T item) { }
        public void Insert(int index, T item) { }
        public bool IsNew(int index) { }
        [System.Diagnostics.ConditionalAttribute("NOP")]
        protected void NotifyListChanged(Castle.Components.DictionaryAdapter.ListProjection<T>.ListChangedType type, int index) { }
        [System.Diagnostics.ConditionalAttribute("NOP")]
        protected void NotifyListReset() { }
        protected virtual void OnInserted(T newValue, int index) { }
        protected virtual bool OnInserting(T value) { }
        protected virtual void OnRemoved(T oldValue, int index) { }
        protected virtual void OnRemoving(T oldValue) { }
        protected virtual void OnReplaced(T oldValue, T newValue, int index) { }
        protected virtual bool OnReplacing(T oldValue, T newValue) { }
        public void RejectChanges() { }
        public virtual bool Remove(T item) { }
        public virtual void RemoveAt(int index) { }
        public void Replace(System.Collections.Generic.IEnumerable<T> items) { }
        public bool ResumeEvents() { }
        public void SuspendEvents() { }
        protected enum ListChangedType<T>
        {
            ItemAdded = 0,
            ItemChanged = 1,
            ItemDeleted = 2,
        }
    }
    public class MemberwiseEqualityHashCodeStrategy : Castle.Components.DictionaryAdapter.DictionaryBehaviorAttribute, Castle.Components.DictionaryAdapter.IDictionaryBehavior, Castle.Components.DictionaryAdapter.IDictionaryEqualityHashCodeStrategy, Castle.Components.DictionaryAdapter.IDictionaryInitializer, System.Collections.Generic.IEqualityComparer<Castle.Components.DictionaryAdapter.IDictionaryAdapter>
    {
        public MemberwiseEqualityHashCodeStrategy() { }
        public bool Equals(Castle.Components.DictionaryAdapter.IDictionaryAdapter adapter1, Castle.Components.DictionaryAdapter.IDictionaryAdapter adapter2) { }
        public int GetHashCode(Castle.Components.DictionaryAdapter.IDictionaryAdapter adapter) { }
        public bool GetHashCode(Castle.Components.DictionaryAdapter.IDictionaryAdapter adapter, out int hashCode) { }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Interface | System.AttributeTargets.All, AllowMultiple=false)]
    public class MultiLevelEditAttribute : Castle.Components.DictionaryAdapter.DictionaryBehaviorAttribute, Castle.Components.DictionaryAdapter.IDictionaryBehavior, Castle.Components.DictionaryAdapter.IDictionaryInitializer
    {
        public MultiLevelEditAttribute() { }
        public void Initialize(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, object[] behaviors) { }
    }
    public class NameValueCollectionAdapter : Castle.Components.DictionaryAdapter.AbstractDictionaryAdapter
    {
        public NameValueCollectionAdapter(System.Collections.Specialized.NameValueCollection nameValues) { }
        public override bool IsReadOnly { get; }
        public override object this[object key] { get; set; }
        public static Castle.Components.DictionaryAdapter.NameValueCollectionAdapter Adapt(System.Collections.Specialized.NameValueCollection nameValues) { }
        public override bool Contains(object key) { }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Property | System.AttributeTargets.Interface | System.AttributeTargets.All, AllowMultiple=false)]
    public class NewGuidAttribute : Castle.Components.DictionaryAdapter.DictionaryBehaviorAttribute, Castle.Components.DictionaryAdapter.IDictionaryBehavior, Castle.Components.DictionaryAdapter.IDictionaryPropertyGetter
    {
        public NewGuidAttribute() { }
        public object GetPropertyValue(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, string key, object storedValue, Castle.Components.DictionaryAdapter.PropertyDescriptor property, bool ifExists) { }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Property | System.AttributeTargets.Interface | System.AttributeTargets.All, AllowMultiple=false)]
    public class OnDemandAttribute : Castle.Components.DictionaryAdapter.DictionaryBehaviorAttribute, Castle.Components.DictionaryAdapter.IDictionaryBehavior, Castle.Components.DictionaryAdapter.IDictionaryPropertyGetter
    {
        public OnDemandAttribute() { }
        public OnDemandAttribute(System.Type type) { }
        public OnDemandAttribute(object value) { }
        public System.Type Type { get; }
        public object Value { get; }
        public object GetPropertyValue(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, string key, object storedValue, Castle.Components.DictionaryAdapter.PropertyDescriptor property, bool ifExists) { }
    }
    public class PropertyChangedEventArgsEx : System.ComponentModel.PropertyChangedEventArgs
    {
        public PropertyChangedEventArgsEx(string propertyName, object oldValue, object newValue) { }
        public object NewValue { get; }
        public object OldValue { get; }
    }
    public class PropertyChangingEventArgsEx : System.ComponentModel.PropertyChangingEventArgs
    {
        public PropertyChangingEventArgsEx(string propertyName, object oldValue, object newValue) { }
        public bool Cancel { get; set; }
        public object NewValue { get; }
        public object OldValue { get; }
    }
    [System.Diagnostics.DebuggerDisplayAttribute("{Property.DeclaringType.FullName,nq}.{PropertyName,nq}")]
    public class PropertyDescriptor : Castle.Components.DictionaryAdapter.IDictionaryBehavior, Castle.Components.DictionaryAdapter.IDictionaryKeyBuilder, Castle.Components.DictionaryAdapter.IDictionaryPropertyGetter, Castle.Components.DictionaryAdapter.IDictionaryPropertySetter
    {
        protected System.Collections.Generic.List<Castle.Components.DictionaryAdapter.IDictionaryBehavior> dictionaryBehaviors;
        public PropertyDescriptor() { }
        public PropertyDescriptor(System.Reflection.PropertyInfo property, object[] annotations) { }
        public PropertyDescriptor(object[] annotations) { }
        public PropertyDescriptor(Castle.Components.DictionaryAdapter.PropertyDescriptor source, bool copyBehaviors) { }
        public object[] Annotations { get; }
        public System.Collections.Generic.IEnumerable<Castle.Components.DictionaryAdapter.IDictionaryBehavior> Behaviors { get; }
        public int ExecutionOrder { get; }
        public System.Collections.IDictionary ExtendedProperties { get; }
        public bool Fetch { get; set; }
        public System.Collections.Generic.IEnumerable<Castle.Components.DictionaryAdapter.IDictionaryPropertyGetter> Getters { get; }
        public bool IfExists { get; set; }
        public System.Collections.Generic.IEnumerable<Castle.Components.DictionaryAdapter.IDictionaryInitializer> Initializers { get; }
        public bool IsDynamicProperty { get; }
        public System.Collections.Generic.IEnumerable<Castle.Components.DictionaryAdapter.IDictionaryKeyBuilder> KeyBuilders { get; }
        public System.Collections.Generic.IEnumerable<Castle.Components.DictionaryAdapter.IDictionaryMetaInitializer> MetaInitializers { get; }
        public System.Reflection.PropertyInfo Property { get; }
        public string PropertyName { get; }
        public System.Type PropertyType { get; }
        public System.Collections.Generic.IEnumerable<Castle.Components.DictionaryAdapter.IDictionaryPropertySetter> Setters { get; }
        public System.Collections.IDictionary State { get; }
        public bool SuppressNotifications { get; set; }
        public System.ComponentModel.TypeConverter TypeConverter { get; }
        public Castle.Components.DictionaryAdapter.PropertyDescriptor AddBehavior(Castle.Components.DictionaryAdapter.IDictionaryBehavior behavior) { }
        public Castle.Components.DictionaryAdapter.PropertyDescriptor AddBehaviors(params Castle.Components.DictionaryAdapter.IDictionaryBehavior[] behaviors) { }
        public Castle.Components.DictionaryAdapter.PropertyDescriptor AddBehaviors(System.Collections.Generic.IEnumerable<Castle.Components.DictionaryAdapter.IDictionaryBehavior> behaviors) { }
        public Castle.Components.DictionaryAdapter.IDictionaryBehavior Copy() { }
        public Castle.Components.DictionaryAdapter.PropertyDescriptor CopyBehaviors(Castle.Components.DictionaryAdapter.PropertyDescriptor other) { }
        public string GetKey(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, string key, Castle.Components.DictionaryAdapter.PropertyDescriptor descriptor) { }
        public object GetPropertyValue(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, string key, object storedValue, Castle.Components.DictionaryAdapter.PropertyDescriptor descriptor, bool ifExists) { }
        public static void MergeBehavior<T>(ref System.Collections.Generic.List<T> dictionaryBehaviors, T behavior)
            where T :  class, Castle.Components.DictionaryAdapter.IDictionaryBehavior { }
        public bool SetPropertyValue(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, string key, ref object value, Castle.Components.DictionaryAdapter.PropertyDescriptor descriptor) { }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Property | System.AttributeTargets.Interface | System.AttributeTargets.All)]
    public class ReferenceAttribute : System.Attribute
    {
        public ReferenceAttribute() { }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Property | System.AttributeTargets.Interface | System.AttributeTargets.All, AllowMultiple=true)]
    public class RemoveIfAttribute : Castle.Components.DictionaryAdapter.DictionaryBehaviorAttribute, Castle.Components.DictionaryAdapter.IDictionaryBehavior, Castle.Components.DictionaryAdapter.IDictionaryPropertySetter
    {
        public RemoveIfAttribute() { }
        public RemoveIfAttribute(params object[] values) { }
        public RemoveIfAttribute(object[] values, System.Type comparerType) { }
        protected RemoveIfAttribute(Castle.Components.DictionaryAdapter.ICondition condition) { }
        public System.Type Condition { set; }
    }
    public class RemoveIfEmptyAttribute : Castle.Components.DictionaryAdapter.RemoveIfAttribute
    {
        public RemoveIfEmptyAttribute() { }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Property | System.AttributeTargets.All, AllowMultiple=false, Inherited=false)]
    public class StringFormatAttribute : Castle.Components.DictionaryAdapter.DictionaryBehaviorAttribute, Castle.Components.DictionaryAdapter.IDictionaryBehavior, Castle.Components.DictionaryAdapter.IDictionaryPropertyGetter
    {
        public StringFormatAttribute(string format, string properties) { }
        public string Format { get; }
        public string Properties { get; }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Property | System.AttributeTargets.All, AllowMultiple=false, Inherited=true)]
    public class StringListAttribute : Castle.Components.DictionaryAdapter.DictionaryBehaviorAttribute, Castle.Components.DictionaryAdapter.IDictionaryBehavior, Castle.Components.DictionaryAdapter.IDictionaryPropertyGetter, Castle.Components.DictionaryAdapter.IDictionaryPropertySetter
    {
        public StringListAttribute() { }
        public char Separator { get; set; }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Property | System.AttributeTargets.Interface | System.AttributeTargets.All, AllowMultiple=false)]
    public class StringStorageAttribute : Castle.Components.DictionaryAdapter.DictionaryBehaviorAttribute, Castle.Components.DictionaryAdapter.IDictionaryBehavior, Castle.Components.DictionaryAdapter.IDictionaryPropertySetter
    {
        public StringStorageAttribute() { }
        public bool SetPropertyValue(Castle.Components.DictionaryAdapter.IDictionaryAdapter dictionaryAdapter, string key, ref object value, Castle.Components.DictionaryAdapter.PropertyDescriptor property) { }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Property | System.AttributeTargets.Interface | System.AttributeTargets.All, AllowMultiple=false, Inherited=true)]
    public class StringValuesAttribute : Castle.Components.DictionaryAdapter.DictionaryBehaviorAttribute, Castle.Components.DictionaryAdapter.IDictionaryBehavior, Castle.Components.DictionaryAdapter.IDictionaryPropertySetter
    {
        public StringValuesAttribute() { }
        public string Format { get; set; }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Property | System.AttributeTargets.All, AllowMultiple=false)]
    public class SuppressNotificationsAttribute : Castle.Components.DictionaryAdapter.DictionaryBehaviorAttribute, Castle.Components.DictionaryAdapter.IDictionaryBehavior, Castle.Components.DictionaryAdapter.IPropertyDescriptorInitializer
    {
        public SuppressNotificationsAttribute() { }
        public void Initialize(Castle.Components.DictionaryAdapter.PropertyDescriptor propertyDescriptor, object[] behaviors) { }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Interface | System.AttributeTargets.All, AllowMultiple=false, Inherited=true)]
    public class TypeKeyPrefixAttribute : Castle.Components.DictionaryAdapter.DictionaryBehaviorAttribute, Castle.Components.DictionaryAdapter.IDictionaryBehavior, Castle.Components.DictionaryAdapter.IDictionaryKeyBuilder
    {
        public TypeKeyPrefixAttribute() { }
    }
    public abstract class VirtualObject<TNode> : Castle.Components.DictionaryAdapter.IVirtual, Castle.Components.DictionaryAdapter.IVirtual<TNode>
    {
        protected VirtualObject() { }
        protected VirtualObject(Castle.Components.DictionaryAdapter.IVirtualSite<TNode> site) { }
        public abstract bool IsReal { get; }
        public event System.EventHandler Realized;
        protected void AddSite(Castle.Components.DictionaryAdapter.IVirtualSite<TNode> site) { }
        protected virtual void OnRealized() { }
        public TNode Realize() { }
        protected void RemoveSite(Castle.Components.DictionaryAdapter.IVirtualSite<TNode> site) { }
        protected abstract bool TryRealize(out TNode node);
    }
    public sealed class VirtualSite<TNode, TMember> : Castle.Components.DictionaryAdapter.IVirtualSite<TNode>, System.IEquatable<Castle.Components.DictionaryAdapter.VirtualSite<TNode, TMember>>
    {
        public VirtualSite(Castle.Components.DictionaryAdapter.IVirtualTarget<TNode, TMember> target, TMember member) { }
        public TMember Member { get; }
        public Castle.Components.DictionaryAdapter.IVirtualTarget<TNode, TMember> Target { get; }
        public override bool Equals(object obj) { }
        public bool Equals(Castle.Components.DictionaryAdapter.VirtualSite<TNode, TMember> other) { }
        public override int GetHashCode() { }
        public void OnRealizing(TNode node) { }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Property | System.AttributeTargets.Interface | System.AttributeTargets.All, AllowMultiple=false)]
    public class VolatileAttribute : System.Attribute
    {
        public VolatileAttribute() { }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Interface | System.AttributeTargets.All)]
    public class XmlDefaultsAttribute : System.Attribute
    {
        public XmlDefaultsAttribute() { }
        public bool IsNullable { get; set; }
        public bool Qualified { get; set; }
    }
    [System.AttributeUsageAttribute(System.AttributeTargets.Property | System.AttributeTargets.Interface | System.AttributeTargets.All, AllowMultiple=true)]
    public class XmlNamespaceAttribute : System.Attribute
    {
        public XmlNamespaceAttribute(string namespaceUri, string prefix) { }
        public bool Default { get; set; }
        public string NamespaceUri { get; }
        public string Prefix { get; }
        public bool Root { get; set; }
    }
}
namespace Castle.Core.Configuration
{
    public abstract class AbstractConfiguration : Castle.Core.Configuration.IConfiguration
    {
        protected AbstractConfiguration() { }
        public virtual Castle.Core.Configuration.ConfigurationAttributeCollection Attributes { get; }
        public virtual Castle.Core.Configuration.ConfigurationCollection Children { get; }
        public string Name { get; set; }
        public string Value { get; set; }
        public virtual object GetValue(System.Type type, object defaultValue) { }
    }
    public class ConfigurationAttributeCollection : System.Collections.Specialized.NameValueCollection
    {
        public ConfigurationAttributeCollection() { }
    }
    public class ConfigurationCollection : System.Collections.Generic.List<Castle.Core.Configuration.IConfiguration>
    {
        public ConfigurationCollection() { }
        public ConfigurationCollection(System.Collections.Generic.IEnumerable<Castle.Core.Configuration.IConfiguration> value) { }
        public Castle.Core.Configuration.IConfiguration this[string name] { get; }
    }
    public interface IConfiguration
    {
        Castle.Core.Configuration.ConfigurationAttributeCollection Attributes { get; }
        Castle.Core.Configuration.ConfigurationCollection Children { get; }
        string Name { get; }
        string Value { get; }
        object GetValue(System.Type type, object defaultValue);
    }
    public class MutableConfiguration : Castle.Core.Configuration.AbstractConfiguration
    {
        public MutableConfiguration(string name) { }
        public MutableConfiguration(string name, string value) { }
        public new string Value { get; set; }
        public Castle.Core.Configuration.MutableConfiguration Attribute(string name, string value) { }
        public static Castle.Core.Configuration.MutableConfiguration Create(string name) { }
        public Castle.Core.Configuration.MutableConfiguration CreateChild(string name) { }
        public Castle.Core.Configuration.MutableConfiguration CreateChild(string name, string value) { }
    }
}
namespace Castle.Core.Configuration.Xml
{
    public class XmlConfigurationDeserializer
    {
        public XmlConfigurationDeserializer() { }
        public Castle.Core.Configuration.IConfiguration Deserialize(System.Xml.XmlNode node) { }
        public static string GetConfigValue(string value) { }
        public static Castle.Core.Configuration.IConfiguration GetDeserializedNode(System.Xml.XmlNode node) { }
        public static bool IsTextNode(System.Xml.XmlNode node) { }
    }
}
namespace Castle.Core
{
    public interface IServiceEnabledComponent
    {
        void Service(System.IServiceProvider provider);
    }
    public interface IServiceProviderEx : System.IServiceProvider
    {
        T GetService<T>()
            where T :  class;
    }
    public interface IServiceProviderExAccessor
    {
        Castle.Core.IServiceProviderEx ServiceProvider { get; }
    }
    public class Pair<TFirst, TSecond> : System.IEquatable<Castle.Core.Pair<TFirst, TSecond>>
    {
        public Pair(TFirst first, TSecond second) { }
        public TFirst First { get; }
        public TSecond Second { get; }
        public bool Equals(Castle.Core.Pair<TFirst, TSecond> other) { }
        public override bool Equals(object obj) { }
        public override int GetHashCode() { }
        public override string ToString() { }
    }
    public class static ProxyServices
    {
        public static bool IsDynamicProxy(System.Type type) { }
    }
    public class ReferenceEqualityComparer<T> : System.Collections.Generic.IEqualityComparer<T>, System.Collections.IEqualityComparer
    {
        public static Castle.Core.ReferenceEqualityComparer<T> Instance { get; }
        public int GetHashCode(object obj) { }
    }
    public sealed class ReflectionBasedDictionaryAdapter : System.Collections.ICollection, System.Collections.IDictionary, System.Collections.IEnumerable
    {
        public ReflectionBasedDictionaryAdapter(object target) { }
        public int Count { get; }
        public bool IsReadOnly { get; }
        public bool IsSynchronized { get; }
        public object this[object key] { get; set; }
        public System.Collections.ICollection Keys { get; }
        public object SyncRoot { get; }
        public System.Collections.ICollection Values { get; }
        public void Add(object key, object value) { }
        public void Clear() { }
        public bool Contains(object key) { }
        public System.Collections.IEnumerator GetEnumerator() { }
        public static void Read(System.Collections.IDictionary targetDictionary, object valuesAsAnonymousObject) { }
        public void Remove(object key) { }
    }
    public sealed class StringObjectDictionaryAdapter : System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string, object>>, System.Collections.Generic.IDictionary<string, object>, System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<string, object>>, System.Collections.IEnumerable
    {
        public StringObjectDictionaryAdapter(System.Collections.IDictionary dictionary) { }
        public int Count { get; }
        public bool IsFixedSize { get; }
        public bool IsReadOnly { get; }
        public bool IsSynchronized { get; }
        public object this[object key] { get; set; }
        public System.Collections.ICollection Keys { get; }
        public object SyncRoot { get; }
        public System.Collections.ICollection Values { get; }
        public void Add(object key, object value) { }
        public void Clear() { }
        public bool Contains(object key) { }
        public void CopyTo(System.Array array, int index) { }
        public System.Collections.IEnumerator GetEnumerator() { }
        public void Remove(object key) { }
    }
}
namespace Castle.Core.Internal
{
    public class static AttributesUtil
    {
        public static T GetAttribute<T>(this System.Type type)
            where T : System.Attribute { }
        public static T GetAttribute<T>(this System.Reflection.MemberInfo member)
            where T : System.Attribute { }
        public static System.AttributeUsageAttribute GetAttributeUsage(this System.Type attributeType) { }
        public static System.Collections.Generic.IEnumerable<T> GetAttributes<T>(this System.Type type)
            where T : System.Attribute { }
        public static System.Collections.Generic.IEnumerable<T> GetAttributes<T>(this System.Reflection.MemberInfo member)
            where T : System.Attribute { }
        public static T GetTypeAttribute<T>(this System.Type type)
            where T : System.Attribute { }
        public static T[] GetTypeAttributes<T>(System.Type type)
            where T : System.Attribute { }
        public static System.Type GetTypeConverter(System.Reflection.MemberInfo member) { }
    }
    public class static CollectionExtensions
    {
        public static bool AreEquivalent<T>(System.Collections.Generic.IList<T> listA, System.Collections.Generic.IList<T> listB) { }
        public static T Find<T>(this T[] items, System.Predicate<T> predicate) { }
        public static T[] FindAll<T>(this T[] items, System.Predicate<T> predicate) { }
        public static int GetContentsHashCode<T>(System.Collections.Generic.IList<T> list) { }
        public static bool IsNullOrEmpty(this System.Collections.IEnumerable @this) { }
    }
    [System.ObsoleteAttribute("Consider using `System.Threading.ReaderWriterLockSlim` instead of `Lock` and rela" +
        "ted types.")]
    public interface ILockHolder : System.IDisposable
    {
        bool LockAcquired { get; }
    }
    [System.ObsoleteAttribute("Consider using `System.Threading.ReaderWriterLockSlim` instead of `Lock` and rela" +
        "ted types.")]
    public interface IUpgradeableLockHolder : Castle.Core.Internal.ILockHolder, System.IDisposable
    {
        Castle.Core.Internal.ILockHolder Upgrade();
        Castle.Core.Internal.ILockHolder Upgrade(bool waitForLock);
    }
    public class InternalsVisible
    {
        public const string ToCastleCore = @"Castle.Core, PublicKey=002400000480000094000000060200000024000052534131000400000100010077F5E87030DADCCCE6902C6ADAB7A987BD69CB5819991531F560785EACFC89B6FCDDF6BB2A00743A7194E454C0273447FC6EEC36474BA8E5A3823147D214298E4F9A631B1AFEE1A51FFEAE4672D498F14B000E3D321453CDD8AC064DE7E1CF4D222B7E81F54D4FD46725370D702A05B48738CC29D09228F1AA722AE1A9CA02FB";
        public const string ToDynamicProxyGenAssembly2 = @"DynamicProxyGenAssembly2, PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac37abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d9266654753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7";
        public InternalsVisible() { }
    }
    [System.ObsoleteAttribute("Consider using `System.Threading.ReaderWriterLockSlim` instead of `Lock` and rela" +
        "ted types.")]
    public abstract class Lock
    {
        protected Lock() { }
        public static Castle.Core.Internal.Lock Create() { }
        public abstract Castle.Core.Internal.ILockHolder ForReading();
        public abstract Castle.Core.Internal.ILockHolder ForReading(bool waitForLock);
        public abstract Castle.Core.Internal.IUpgradeableLockHolder ForReadingUpgradeable();
        public abstract Castle.Core.Internal.IUpgradeableLockHolder ForReadingUpgradeable(bool waitForLock);
        public abstract Castle.Core.Internal.ILockHolder ForWriting();
        public abstract Castle.Core.Internal.ILockHolder ForWriting(bool waitForLock);
    }
}
namespace Castle.Core.Logging
{
    public abstract class AbstractExtendedLoggerFactory : Castle.Core.Logging.IExtendedLoggerFactory, Castle.Core.Logging.ILoggerFactory
    {
        protected AbstractExtendedLoggerFactory() { }
        public virtual Castle.Core.Logging.IExtendedLogger Create(System.Type type) { }
        public abstract Castle.Core.Logging.IExtendedLogger Create(string name);
        public virtual Castle.Core.Logging.IExtendedLogger Create(System.Type type, Castle.Core.Logging.LoggerLevel level) { }
        public abstract Castle.Core.Logging.IExtendedLogger Create(string name, Castle.Core.Logging.LoggerLevel level);
        protected static System.IO.FileInfo GetConfigFile(string fileName) { }
    }
    public abstract class AbstractLoggerFactory : Castle.Core.Logging.ILoggerFactory
    {
        protected AbstractLoggerFactory() { }
        public virtual Castle.Core.Logging.ILogger Create(System.Type type) { }
        public virtual Castle.Core.Logging.ILogger Create(System.Type type, Castle.Core.Logging.LoggerLevel level) { }
        public abstract Castle.Core.Logging.ILogger Create(string name);
        public abstract Castle.Core.Logging.ILogger Create(string name, Castle.Core.Logging.LoggerLevel level);
        protected static System.IO.FileInfo GetConfigFile(string fileName) { }
    }
    public class ConsoleFactory : Castle.Core.Logging.ILoggerFactory
    {
        public ConsoleFactory() { }
        public ConsoleFactory(Castle.Core.Logging.LoggerLevel level) { }
        public Castle.Core.Logging.ILogger Create(System.Type type) { }
        public Castle.Core.Logging.ILogger Create(string name) { }
        public Castle.Core.Logging.ILogger Create(System.Type type, Castle.Core.Logging.LoggerLevel level) { }
        public Castle.Core.Logging.ILogger Create(string name, Castle.Core.Logging.LoggerLevel level) { }
    }
    public class ConsoleLogger : Castle.Core.Logging.LevelFilteredLogger
    {
        public ConsoleLogger() { }
        public ConsoleLogger(Castle.Core.Logging.LoggerLevel logLevel) { }
        public ConsoleLogger(string name) { }
        public ConsoleLogger(string name, Castle.Core.Logging.LoggerLevel logLevel) { }
        public override Castle.Core.Logging.ILogger CreateChildLogger(string loggerName) { }
        protected override void Log(Castle.Core.Logging.LoggerLevel loggerLevel, string loggerName, string message, System.Exception exception) { }
    }
    public interface IContextProperties
    {
        object this[string key] { get; set; }
    }
    public interface IContextStack
    {
        int Count { get; }
        void Clear();
        string Pop();
        System.IDisposable Push(string message);
    }
    public interface IContextStacks
    {
        Castle.Core.Logging.IContextStack this[string key] { get; }
    }
    public interface IExtendedLogger : Castle.Core.Logging.ILogger
    {
        Castle.Core.Logging.IContextProperties GlobalProperties { get; }
        Castle.Core.Logging.IContextProperties ThreadProperties { get; }
        Castle.Core.Logging.IContextStacks ThreadStacks { get; }
    }
    public interface IExtendedLoggerFactory : Castle.Core.Logging.ILoggerFactory
    {
        Castle.Core.Logging.IExtendedLogger Create(System.Type type);
        Castle.Core.Logging.IExtendedLogger Create(string name);
        Castle.Core.Logging.IExtendedLogger Create(System.Type type, Castle.Core.Logging.LoggerLevel level);
        Castle.Core.Logging.IExtendedLogger Create(string name, Castle.Core.Logging.LoggerLevel level);
    }
    public interface ILogger
    {
        bool IsDebugEnabled { get; }
        bool IsErrorEnabled { get; }
        bool IsFatalEnabled { get; }
        bool IsInfoEnabled { get; }
        bool IsTraceEnabled { get; }
        bool IsWarnEnabled { get; }
        Castle.Core.Logging.ILogger CreateChildLogger(string loggerName);
        void Debug(string message);
        void Debug(System.Func<string> messageFactory);
        void Debug(string message, System.Exception exception);
        void DebugFormat(string format, params object[] args);
        void DebugFormat(System.Exception exception, string format, params object[] args);
        void DebugFormat(System.IFormatProvider formatProvider, string format, params object[] args);
        void DebugFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args);
        void Error(string message);
        void Error(System.Func<string> messageFactory);
        void Error(string message, System.Exception exception);
        void ErrorFormat(string format, params object[] args);
        void ErrorFormat(System.Exception exception, string format, params object[] args);
        void ErrorFormat(System.IFormatProvider formatProvider, string format, params object[] args);
        void ErrorFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args);
        void Fatal(string message);
        void Fatal(System.Func<string> messageFactory);
        void Fatal(string message, System.Exception exception);
        void FatalFormat(string format, params object[] args);
        void FatalFormat(System.Exception exception, string format, params object[] args);
        void FatalFormat(System.IFormatProvider formatProvider, string format, params object[] args);
        void FatalFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args);
        void Info(string message);
        void Info(System.Func<string> messageFactory);
        void Info(string message, System.Exception exception);
        void InfoFormat(string format, params object[] args);
        void InfoFormat(System.Exception exception, string format, params object[] args);
        void InfoFormat(System.IFormatProvider formatProvider, string format, params object[] args);
        void InfoFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args);
        void Trace(string message);
        void Trace(System.Func<string> messageFactory);
        void Trace(string message, System.Exception exception);
        void TraceFormat(string format, params object[] args);
        void TraceFormat(System.Exception exception, string format, params object[] args);
        void TraceFormat(System.IFormatProvider formatProvider, string format, params object[] args);
        void TraceFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args);
        void Warn(string message);
        void Warn(System.Func<string> messageFactory);
        void Warn(string message, System.Exception exception);
        void WarnFormat(string format, params object[] args);
        void WarnFormat(System.Exception exception, string format, params object[] args);
        void WarnFormat(System.IFormatProvider formatProvider, string format, params object[] args);
        void WarnFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args);
    }
    public interface ILoggerFactory
    {
        Castle.Core.Logging.ILogger Create(System.Type type);
        Castle.Core.Logging.ILogger Create(string name);
        Castle.Core.Logging.ILogger Create(System.Type type, Castle.Core.Logging.LoggerLevel level);
        Castle.Core.Logging.ILogger Create(string name, Castle.Core.Logging.LoggerLevel level);
    }
    public abstract class LevelFilteredLogger : Castle.Core.Logging.ILogger
    {
        protected LevelFilteredLogger() { }
        protected LevelFilteredLogger(string name) { }
        protected LevelFilteredLogger(Castle.Core.Logging.LoggerLevel loggerLevel) { }
        protected LevelFilteredLogger(string loggerName, Castle.Core.Logging.LoggerLevel loggerLevel) { }
        public bool IsDebugEnabled { get; }
        public bool IsErrorEnabled { get; }
        public bool IsFatalEnabled { get; }
        public bool IsInfoEnabled { get; }
        public bool IsTraceEnabled { get; }
        public bool IsWarnEnabled { get; }
        public Castle.Core.Logging.LoggerLevel Level { get; set; }
        public string Name { get; }
        protected void ChangeName(string newName) { }
        public abstract Castle.Core.Logging.ILogger CreateChildLogger(string loggerName);
        public void Debug(string message) { }
        public void Debug(System.Func<string> messageFactory) { }
        public void Debug(string message, System.Exception exception) { }
        public void DebugFormat(string format, params object[] args) { }
        public void DebugFormat(System.Exception exception, string format, params object[] args) { }
        public void DebugFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void DebugFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void Error(string message) { }
        public void Error(System.Func<string> messageFactory) { }
        public void Error(string message, System.Exception exception) { }
        public void ErrorFormat(string format, params object[] args) { }
        public void ErrorFormat(System.Exception exception, string format, params object[] args) { }
        public void ErrorFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void ErrorFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void Fatal(string message) { }
        public void Fatal(System.Func<string> messageFactory) { }
        public void Fatal(string message, System.Exception exception) { }
        public void FatalFormat(string format, params object[] args) { }
        public void FatalFormat(System.Exception exception, string format, params object[] args) { }
        public void FatalFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void FatalFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void Info(string message) { }
        public void Info(System.Func<string> messageFactory) { }
        public void Info(string message, System.Exception exception) { }
        public void InfoFormat(string format, params object[] args) { }
        public void InfoFormat(System.Exception exception, string format, params object[] args) { }
        public void InfoFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void InfoFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
        protected abstract void Log(Castle.Core.Logging.LoggerLevel loggerLevel, string loggerName, string message, System.Exception exception);
        public void Trace(string message) { }
        public void Trace(System.Func<string> messageFactory) { }
        public void Trace(string message, System.Exception exception) { }
        public void TraceFormat(string format, params object[] args) { }
        public void TraceFormat(System.Exception exception, string format, params object[] args) { }
        public void TraceFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void TraceFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void Warn(string message) { }
        public void Warn(System.Func<string> messageFactory) { }
        public void Warn(string message, System.Exception exception) { }
        public void WarnFormat(string format, params object[] args) { }
        public void WarnFormat(System.Exception exception, string format, params object[] args) { }
        public void WarnFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void WarnFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
    }
    public class LoggerException : System.Exception
    {
        public LoggerException() { }
        public LoggerException(string message) { }
        public LoggerException(string message, System.Exception innerException) { }
    }
    public enum LoggerLevel
    {
        Off = 0,
        Fatal = 1,
        Error = 2,
        Warn = 3,
        Info = 4,
        Debug = 5,
        Trace = 6,
    }
    public class NullLogFactory : Castle.Core.Logging.AbstractLoggerFactory
    {
        public NullLogFactory() { }
        public override Castle.Core.Logging.ILogger Create(string name) { }
        public override Castle.Core.Logging.ILogger Create(string name, Castle.Core.Logging.LoggerLevel level) { }
    }
    public class NullLogger : Castle.Core.Logging.IExtendedLogger, Castle.Core.Logging.ILogger
    {
        public static readonly Castle.Core.Logging.NullLogger Instance;
        public NullLogger() { }
        public Castle.Core.Logging.IContextProperties GlobalProperties { get; }
        public bool IsDebugEnabled { get; }
        public bool IsErrorEnabled { get; }
        public bool IsFatalEnabled { get; }
        public bool IsInfoEnabled { get; }
        public bool IsTraceEnabled { get; }
        public bool IsWarnEnabled { get; }
        public Castle.Core.Logging.IContextProperties ThreadProperties { get; }
        public Castle.Core.Logging.IContextStacks ThreadStacks { get; }
        public Castle.Core.Logging.ILogger CreateChildLogger(string loggerName) { }
        public void Debug(string message) { }
        public void Debug(System.Func<string> messageFactory) { }
        public void Debug(string message, System.Exception exception) { }
        public void DebugFormat(string format, params object[] args) { }
        public void DebugFormat(System.Exception exception, string format, params object[] args) { }
        public void DebugFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void DebugFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void Error(string message) { }
        public void Error(System.Func<string> messageFactory) { }
        public void Error(string message, System.Exception exception) { }
        public void ErrorFormat(string format, params object[] args) { }
        public void ErrorFormat(System.Exception exception, string format, params object[] args) { }
        public void ErrorFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void ErrorFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void Fatal(string message) { }
        public void Fatal(System.Func<string> messageFactory) { }
        public void Fatal(string message, System.Exception exception) { }
        public void FatalFormat(string format, params object[] args) { }
        public void FatalFormat(System.Exception exception, string format, params object[] args) { }
        public void FatalFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void FatalFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void Info(string message) { }
        public void Info(System.Func<string> messageFactory) { }
        public void Info(string message, System.Exception exception) { }
        public void InfoFormat(string format, params object[] args) { }
        public void InfoFormat(System.Exception exception, string format, params object[] args) { }
        public void InfoFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void InfoFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void Trace(string message) { }
        public void Trace(System.Func<string> messageFactory) { }
        public void Trace(string message, System.Exception exception) { }
        public void TraceFormat(string format, params object[] args) { }
        public void TraceFormat(System.Exception exception, string format, params object[] args) { }
        public void TraceFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void TraceFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void Warn(string message) { }
        public void Warn(System.Func<string> messageFactory) { }
        public void Warn(string message, System.Exception exception) { }
        public void WarnFormat(string format, params object[] args) { }
        public void WarnFormat(System.Exception exception, string format, params object[] args) { }
        public void WarnFormat(System.IFormatProvider formatProvider, string format, params object[] args) { }
        public void WarnFormat(System.Exception exception, System.IFormatProvider formatProvider, string format, params object[] args) { }
    }
    public class StreamLogger : Castle.Core.Logging.LevelFilteredLogger, System.IDisposable
    {
        public StreamLogger(string name, System.IO.Stream stream) { }
        public StreamLogger(string name, System.IO.Stream stream, System.Text.Encoding encoding) { }
        public StreamLogger(string name, System.IO.Stream stream, System.Text.Encoding encoding, int bufferSize) { }
        protected StreamLogger(string name, System.IO.StreamWriter writer) { }
        public override Castle.Core.Logging.ILogger CreateChildLogger(string loggerName) { }
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        protected override void Finalize() { }
        protected override void Log(Castle.Core.Logging.LoggerLevel loggerLevel, string loggerName, string message, System.Exception exception) { }
    }
    public class StreamLoggerFactory : Castle.Core.Logging.AbstractLoggerFactory
    {
        public StreamLoggerFactory() { }
        public override Castle.Core.Logging.ILogger Create(string name) { }
        public override Castle.Core.Logging.ILogger Create(string name, Castle.Core.Logging.LoggerLevel level) { }
    }
    public class TraceLogger : Castle.Core.Logging.LevelFilteredLogger
    {
        public TraceLogger(string name) { }
        public TraceLogger(string name, Castle.Core.Logging.LoggerLevel level) { }
        public override Castle.Core.Logging.ILogger CreateChildLogger(string loggerName) { }
        protected override void Log(Castle.Core.Logging.LoggerLevel loggerLevel, string loggerName, string message, System.Exception exception) { }
    }
    public class TraceLoggerFactory : Castle.Core.Logging.AbstractLoggerFactory
    {
        public TraceLoggerFactory() { }
        public TraceLoggerFactory(Castle.Core.Logging.LoggerLevel level) { }
        public override Castle.Core.Logging.ILogger Create(string name) { }
        public override Castle.Core.Logging.ILogger Create(string name, Castle.Core.Logging.LoggerLevel level) { }
    }
}
namespace Castle.Core.Resource
{
    public abstract class AbstractResource : Castle.Core.Resource.IResource, System.IDisposable
    {
        protected static readonly string DefaultBasePath;
        protected AbstractResource() { }
        public virtual string FileBasePath { get; }
        public abstract Castle.Core.Resource.IResource CreateRelative(string relativePath);
        public void Dispose() { }
        protected virtual void Dispose(bool disposing) { }
        public abstract System.IO.TextReader GetStreamReader();
        public abstract System.IO.TextReader GetStreamReader(System.Text.Encoding encoding);
    }
    public abstract class AbstractStreamResource : Castle.Core.Resource.AbstractResource
    {
        protected AbstractStreamResource() { }
        public Castle.Core.Resource.StreamFactory CreateStream { get; set; }
        protected override void Finalize() { }
        public override System.IO.TextReader GetStreamReader() { }
        public override System.IO.TextReader GetStreamReader(System.Text.Encoding encoding) { }
    }
    public class AssemblyBundleResource : Castle.Core.Resource.AbstractResource
    {
        public AssemblyBundleResource(Castle.Core.Resource.CustomUri resource) { }
        public override Castle.Core.Resource.IResource CreateRelative(string relativePath) { }
        public override System.IO.TextReader GetStreamReader() { }
        public override System.IO.TextReader GetStreamReader(System.Text.Encoding encoding) { }
    }
    public class AssemblyResource : Castle.Core.Resource.AbstractStreamResource
    {
        public AssemblyResource(Castle.Core.Resource.CustomUri resource) { }
        public AssemblyResource(Castle.Core.Resource.CustomUri resource, string basePath) { }
        public AssemblyResource(string resource) { }
        public override Castle.Core.Resource.IResource CreateRelative(string relativePath) { }
        public override string ToString() { }
    }
    public class AssemblyResourceFactory : Castle.Core.Resource.IResourceFactory
    {
        public AssemblyResourceFactory() { }
        public bool Accept(Castle.Core.Resource.CustomUri uri) { }
        public Castle.Core.Resource.IResource Create(Castle.Core.Resource.CustomUri uri) { }
        public Castle.Core.Resource.IResource Create(Castle.Core.Resource.CustomUri uri, string basePath) { }
    }
    public sealed class CustomUri
    {
        public static readonly string SchemeDelimiter;
        public static readonly string UriSchemeAssembly;
        public static readonly string UriSchemeFile;
        public CustomUri(string resourceIdentifier) { }
        public string Host { get; }
        public bool IsAssembly { get; }
        public bool IsFile { get; }
        public bool IsUnc { get; }
        public string Path { get; }
        public string Scheme { get; }
    }
    public class FileResource : Castle.Core.Resource.AbstractStreamResource
    {
        public FileResource(Castle.Core.Resource.CustomUri resource) { }
        public FileResource(Castle.Core.Resource.CustomUri resource, string basePath) { }
        public FileResource(string resourceName) { }
        public FileResource(string resourceName, string basePath) { }
        public override string FileBasePath { get; }
        public override Castle.Core.Resource.IResource CreateRelative(string relativePath) { }
        public override string ToString() { }
    }
    public class FileResourceFactory : Castle.Core.Resource.IResourceFactory
    {
        public FileResourceFactory() { }
        public bool Accept(Castle.Core.Resource.CustomUri uri) { }
        public Castle.Core.Resource.IResource Create(Castle.Core.Resource.CustomUri uri) { }
        public Castle.Core.Resource.IResource Create(Castle.Core.Resource.CustomUri uri, string basePath) { }
    }
    public interface IResource : System.IDisposable
    {
        string FileBasePath { get; }
        Castle.Core.Resource.IResource CreateRelative(string relativePath);
        System.IO.TextReader GetStreamReader();
        System.IO.TextReader GetStreamReader(System.Text.Encoding encoding);
    }
    public interface IResourceFactory
    {
        bool Accept(Castle.Core.Resource.CustomUri uri);
        Castle.Core.Resource.IResource Create(Castle.Core.Resource.CustomUri uri);
        Castle.Core.Resource.IResource Create(Castle.Core.Resource.CustomUri uri, string basePath);
    }
    public class ResourceException : System.Exception
    {
        public ResourceException() { }
        public ResourceException(string message) { }
        public ResourceException(string message, System.Exception innerException) { }
    }
    public class StaticContentResource : Castle.Core.Resource.AbstractResource
    {
        public StaticContentResource(string contents) { }
        public override Castle.Core.Resource.IResource CreateRelative(string relativePath) { }
        public override System.IO.TextReader GetStreamReader() { }
        public override System.IO.TextReader GetStreamReader(System.Text.Encoding encoding) { }
    }
    public delegate System.IO.Stream StreamFactory();
    public class UncResource : Castle.Core.Resource.AbstractStreamResource
    {
        public UncResource(Castle.Core.Resource.CustomUri resource) { }
        public UncResource(Castle.Core.Resource.CustomUri resource, string basePath) { }
        public UncResource(string resourceName) { }
        public UncResource(string resourceName, string basePath) { }
        public override string FileBasePath { get; }
        public override Castle.Core.Resource.IResource CreateRelative(string relativePath) { }
        public override string ToString() { }
    }
    public class UncResourceFactory : Castle.Core.Resource.IResourceFactory
    {
        public UncResourceFactory() { }
        public bool Accept(Castle.Core.Resource.CustomUri uri) { }
        public Castle.Core.Resource.IResource Create(Castle.Core.Resource.CustomUri uri) { }
        public Castle.Core.Resource.IResource Create(Castle.Core.Resource.CustomUri uri, string basePath) { }
    }
}
namespace Castle.DynamicProxy
{
    public abstract class AbstractInvocation : Castle.DynamicProxy.IInvocation
    {
        protected readonly object proxyObject;
        protected AbstractInvocation(object proxy, Castle.DynamicProxy.IInterceptor[] interceptors, System.Reflection.MethodInfo proxiedMethod, object[] arguments) { }
        public object[] Arguments { get; }
        public System.Type[] GenericArguments { get; }
        public abstract object InvocationTarget { get; }
        public System.Reflection.MethodInfo Method { get; }
        public abstract System.Reflection.MethodInfo MethodInvocationTarget { get; }
        public object Proxy { get; }
        public object ReturnValue { get; set; }
        public abstract System.Type TargetType { get; }
        public Castle.DynamicProxy.IInvocationProceedInfo CaptureProceedInfo() { }
        public object GetArgumentValue(int index) { }
        public System.Reflection.MethodInfo GetConcreteMethod() { }
        public System.Reflection.MethodInfo GetConcreteMethodInvocationTarget() { }
        protected abstract void InvokeMethodOnTarget();
        public void Proceed() { }
        public void SetArgumentValue(int index, object value) { }
        public void SetGenericMethodArguments(System.Type[] arguments) { }
        protected void ThrowOnNoTarget() { }
    }
    public class AllMethodsHook : Castle.DynamicProxy.IProxyGenerationHook
    {
        protected static readonly System.Collections.Generic.ICollection<System.Type> SkippedTypes;
        public AllMethodsHook() { }
        public override bool Equals(object obj) { }
        public override int GetHashCode() { }
        public virtual void MethodsInspected() { }
        public virtual void NonProxyableMemberNotification(System.Type type, System.Reflection.MemberInfo memberInfo) { }
        public virtual bool ShouldInterceptMethod(System.Type type, System.Reflection.MethodInfo methodInfo) { }
    }
    public class CustomAttributeInfo : System.IEquatable<Castle.DynamicProxy.CustomAttributeInfo>
    {
        public CustomAttributeInfo(System.Reflection.ConstructorInfo constructor, object[] constructorArgs, System.Reflection.PropertyInfo[] namedProperties, object[] propertyValues, System.Reflection.FieldInfo[] namedFields, object[] fieldValues) { }
        public CustomAttributeInfo(System.Reflection.ConstructorInfo constructor, object[] constructorArgs, System.Reflection.PropertyInfo[] namedProperties, object[] propertyValues) { }
        public CustomAttributeInfo(System.Reflection.ConstructorInfo constructor, object[] constructorArgs, System.Reflection.FieldInfo[] namedFields, object[] fieldValues) { }
        public CustomAttributeInfo(System.Reflection.ConstructorInfo constructor, object[] constructorArgs) { }
        public bool Equals(Castle.DynamicProxy.CustomAttributeInfo other) { }
        public override bool Equals(object obj) { }
        public static Castle.DynamicProxy.CustomAttributeInfo FromExpression(System.Linq.Expressions.Expression<System.Func<System.Attribute>> expression) { }
        public override int GetHashCode() { }
    }
    public class DefaultProxyBuilder : Castle.DynamicProxy.IProxyBuilder
    {
        public DefaultProxyBuilder() { }
        public DefaultProxyBuilder(Castle.DynamicProxy.ModuleScope scope) { }
        public Castle.Core.Logging.ILogger Logger { get; set; }
        public Castle.DynamicProxy.ModuleScope ModuleScope { get; }
        public System.Type CreateClassProxyType(System.Type classToProxy, System.Type[] additionalInterfacesToProxy, Castle.DynamicProxy.ProxyGenerationOptions options) { }
        public System.Type CreateClassProxyTypeWithTarget(System.Type classToProxy, System.Type[] additionalInterfacesToProxy, Castle.DynamicProxy.ProxyGenerationOptions options) { }
        public System.Type CreateInterfaceProxyTypeWithTarget(System.Type interfaceToProxy, System.Type[] additionalInterfacesToProxy, System.Type targetType, Castle.DynamicProxy.ProxyGenerationOptions options) { }
        public System.Type CreateInterfaceProxyTypeWithTargetInterface(System.Type interfaceToProxy, System.Type[] additionalInterfacesToProxy, Castle.DynamicProxy.ProxyGenerationOptions options) { }
        public System.Type CreateInterfaceProxyTypeWithoutTarget(System.Type interfaceToProxy, System.Type[] additionalInterfacesToProxy, Castle.DynamicProxy.ProxyGenerationOptions options) { }
    }
    public interface IChangeProxyTarget
    {
        void ChangeInvocationTarget(object target);
        [System.ObsoleteAttribute("Use ((IProxyTargetAccessor)invocation.Proxy).DynProxySetTarget(target) instead.")]
        void ChangeProxyTarget(object target);
    }
    public interface IInterceptor
    {
        void Intercept(Castle.DynamicProxy.IInvocation invocation);
    }
    public interface IInterceptorSelector
    {
        Castle.DynamicProxy.IInterceptor[] SelectInterceptors(System.Type type, System.Reflection.MethodInfo method, Castle.DynamicProxy.IInterceptor[] interceptors);
    }
    public interface IInvocation
    {
        object[] Arguments { get; }
        System.Type[] GenericArguments { get; }
        object InvocationTarget { get; }
        System.Reflection.MethodInfo Method { get; }
        System.Reflection.MethodInfo MethodInvocationTarget { get; }
        object Proxy { get; }
        object ReturnValue { get; set; }
        System.Type TargetType { get; }
        Castle.DynamicProxy.IInvocationProceedInfo CaptureProceedInfo();
        object GetArgumentValue(int index);
        System.Reflection.MethodInfo GetConcreteMethod();
        System.Reflection.MethodInfo GetConcreteMethodInvocationTarget();
        void Proceed();
        void SetArgumentValue(int index, object value);
    }
    public interface IInvocationProceedInfo
    {
        void Invoke();
    }
    public interface IProxyBuilder
    {
        Castle.Core.Logging.ILogger Logger { get; set; }
        Castle.DynamicProxy.ModuleScope ModuleScope { get; }
        System.Type CreateClassProxyType(System.Type classToProxy, System.Type[] additionalInterfacesToProxy, Castle.DynamicProxy.ProxyGenerationOptions options);
        System.Type CreateClassProxyTypeWithTarget(System.Type classToProxy, System.Type[] additionalInterfacesToProxy, Castle.DynamicProxy.ProxyGenerationOptions options);
        System.Type CreateInterfaceProxyTypeWithTarget(System.Type interfaceToProxy, System.Type[] additionalInterfacesToProxy, System.Type targetType, Castle.DynamicProxy.ProxyGenerationOptions options);
        System.Type CreateInterfaceProxyTypeWithTargetInterface(System.Type interfaceToProxy, System.Type[] additionalInterfacesToProxy, Castle.DynamicProxy.ProxyGenerationOptions options);
        System.Type CreateInterfaceProxyTypeWithoutTarget(System.Type interfaceToProxy, System.Type[] additionalInterfacesToProxy, Castle.DynamicProxy.ProxyGenerationOptions options);
    }
    public interface IProxyGenerationHook
    {
        void MethodsInspected();
        void NonProxyableMemberNotification(System.Type type, System.Reflection.MemberInfo memberInfo);
        bool ShouldInterceptMethod(System.Type type, System.Reflection.MethodInfo methodInfo);
    }
    [System.CLSCompliantAttribute(true)]
    public interface IProxyGenerator
    {
        Castle.Core.Logging.ILogger Logger { get; set; }
        Castle.DynamicProxy.IProxyBuilder ProxyBuilder { get; }
        TClass CreateClassProxy<TClass>(params Castle.DynamicProxy.IInterceptor[] interceptors)
            where TClass :  class;
        TClass CreateClassProxy<TClass>(Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors)
            where TClass :  class;
        object CreateClassProxy(System.Type classToProxy, System.Type[] additionalInterfacesToProxy, params Castle.DynamicProxy.IInterceptor[] interceptors);
        object CreateClassProxy(System.Type classToProxy, Castle.DynamicProxy.ProxyGenerationOptions options, object[] constructorArguments, params Castle.DynamicProxy.IInterceptor[] interceptors);
        object CreateClassProxy(System.Type classToProxy, object[] constructorArguments, params Castle.DynamicProxy.IInterceptor[] interceptors);
        object CreateClassProxy(System.Type classToProxy, params Castle.DynamicProxy.IInterceptor[] interceptors);
        object CreateClassProxy(System.Type classToProxy, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors);
        object CreateClassProxy(System.Type classToProxy, System.Type[] additionalInterfacesToProxy, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors);
        object CreateClassProxy(System.Type classToProxy, System.Type[] additionalInterfacesToProxy, Castle.DynamicProxy.ProxyGenerationOptions options, object[] constructorArguments, params Castle.DynamicProxy.IInterceptor[] interceptors);
        TClass CreateClassProxyWithTarget<TClass>(TClass target, params Castle.DynamicProxy.IInterceptor[] interceptors)
            where TClass :  class;
        TClass CreateClassProxyWithTarget<TClass>(TClass target, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors)
            where TClass :  class;
        object CreateClassProxyWithTarget(System.Type classToProxy, System.Type[] additionalInterfacesToProxy, object target, params Castle.DynamicProxy.IInterceptor[] interceptors);
        object CreateClassProxyWithTarget(System.Type classToProxy, object target, Castle.DynamicProxy.ProxyGenerationOptions options, object[] constructorArguments, params Castle.DynamicProxy.IInterceptor[] interceptors);
        object CreateClassProxyWithTarget(System.Type classToProxy, object target, object[] constructorArguments, params Castle.DynamicProxy.IInterceptor[] interceptors);
        object CreateClassProxyWithTarget(System.Type classToProxy, object target, params Castle.DynamicProxy.IInterceptor[] interceptors);
        object CreateClassProxyWithTarget(System.Type classToProxy, object target, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors);
        object CreateClassProxyWithTarget(System.Type classToProxy, System.Type[] additionalInterfacesToProxy, object target, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors);
        object CreateClassProxyWithTarget(System.Type classToProxy, System.Type[] additionalInterfacesToProxy, object target, Castle.DynamicProxy.ProxyGenerationOptions options, object[] constructorArguments, params Castle.DynamicProxy.IInterceptor[] interceptors);
        TInterface CreateInterfaceProxyWithTarget<TInterface>(TInterface target, params Castle.DynamicProxy.IInterceptor[] interceptors)
            where TInterface :  class;
        TInterface CreateInterfaceProxyWithTarget<TInterface>(TInterface target, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors)
            where TInterface :  class;
        object CreateInterfaceProxyWithTarget(System.Type interfaceToProxy, object target, params Castle.DynamicProxy.IInterceptor[] interceptors);
        object CreateInterfaceProxyWithTarget(System.Type interfaceToProxy, object target, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors);
        object CreateInterfaceProxyWithTarget(System.Type interfaceToProxy, System.Type[] additionalInterfacesToProxy, object target, params Castle.DynamicProxy.IInterceptor[] interceptors);
        object CreateInterfaceProxyWithTarget(System.Type interfaceToProxy, System.Type[] additionalInterfacesToProxy, object target, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors);
        object CreateInterfaceProxyWithTargetInterface(System.Type interfaceToProxy, object target, params Castle.DynamicProxy.IInterceptor[] interceptors);
        TInterface CreateInterfaceProxyWithTargetInterface<TInterface>(TInterface target, params Castle.DynamicProxy.IInterceptor[] interceptors)
            where TInterface :  class;
        TInterface CreateInterfaceProxyWithTargetInterface<TInterface>(TInterface target, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors)
            where TInterface :  class;
        object CreateInterfaceProxyWithTargetInterface(System.Type interfaceToProxy, System.Type[] additionalInterfacesToProxy, object target, params Castle.DynamicProxy.IInterceptor[] interceptors);
        object CreateInterfaceProxyWithTargetInterface(System.Type interfaceToProxy, object target, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors);
        object CreateInterfaceProxyWithTargetInterface(System.Type interfaceToProxy, System.Type[] additionalInterfacesToProxy, object target, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors);
        TInterface CreateInterfaceProxyWithoutTarget<TInterface>(Castle.DynamicProxy.IInterceptor interceptor)
            where TInterface :  class;
        TInterface CreateInterfaceProxyWithoutTarget<TInterface>(params Castle.DynamicProxy.IInterceptor[] interceptors)
            where TInterface :  class;
        TInterface CreateInterfaceProxyWithoutTarget<TInterface>(Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors)
            where TInterface :  class;
        object CreateInterfaceProxyWithoutTarget(System.Type interfaceToProxy, Castle.DynamicProxy.IInterceptor interceptor);
        object CreateInterfaceProxyWithoutTarget(System.Type interfaceToProxy, params Castle.DynamicProxy.IInterceptor[] interceptors);
        object CreateInterfaceProxyWithoutTarget(System.Type interfaceToProxy, System.Type[] additionalInterfacesToProxy, params Castle.DynamicProxy.IInterceptor[] interceptors);
        object CreateInterfaceProxyWithoutTarget(System.Type interfaceToProxy, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors);
        object CreateInterfaceProxyWithoutTarget(System.Type interfaceToProxy, System.Type[] additionalInterfacesToProxy, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors);
    }
    public interface IProxyTargetAccessor
    {
        object DynProxyGetTarget();
        void DynProxySetTarget(object target);
        Castle.DynamicProxy.IInterceptor[] GetInterceptors();
    }
    public class InvalidMixinConfigurationException : System.Exception
    {
        public InvalidMixinConfigurationException(string message) { }
        public InvalidMixinConfigurationException(string message, System.Exception innerException) { }
    }
    public class InvalidProxyConstructorArgumentsException : System.ArgumentException
    {
        public InvalidProxyConstructorArgumentsException(string message, System.Type proxyType, System.Type classToProxy) { }
        public System.Type ClassToProxy { get; }
        public System.Type ProxyType { get; }
    }
    public class MixinData
    {
        public MixinData(System.Collections.Generic.IEnumerable<object> mixinInstances) { }
        public System.Collections.Generic.IEnumerable<System.Type> MixinInterfaces { get; }
        public System.Collections.Generic.IEnumerable<object> Mixins { get; }
        public bool ContainsMixin(System.Type mixinInterfaceType) { }
        public override bool Equals(object obj) { }
        public override int GetHashCode() { }
        public object GetMixinInstance(System.Type mixinInterfaceType) { }
        public int GetMixinPosition(System.Type mixinInterfaceType) { }
    }
    public class ModuleScope
    {
        public static readonly string DEFAULT_ASSEMBLY_NAME;
        public static readonly string DEFAULT_FILE_NAME;
        public ModuleScope() { }
        public ModuleScope(bool savePhysicalAssembly) { }
        public ModuleScope(bool savePhysicalAssembly, bool disableSignedModule) { }
        public ModuleScope(bool savePhysicalAssembly, bool disableSignedModule, string strongAssemblyName, string strongModulePath, string weakAssemblyName, string weakModulePath) { }
        public ModuleScope(bool savePhysicalAssembly, bool disableSignedModule, Castle.DynamicProxy.Generators.INamingScope namingScope, string strongAssemblyName, string strongModulePath, string weakAssemblyName, string weakModulePath) { }
        [System.ObsoleteAttribute("Exposes a component that is intended for internal use only.")]
        public Castle.Core.Internal.Lock Lock { get; }
        public Castle.DynamicProxy.Generators.INamingScope NamingScope { get; }
        public System.Reflection.Emit.ModuleBuilder StrongNamedModule { get; }
        public string StrongNamedModuleName { get; }
        public System.Reflection.Emit.ModuleBuilder WeakNamedModule { get; }
        public string WeakNamedModuleName { get; }
        [System.ObsoleteAttribute("Exposes a component that is intended for internal use only.")]
        public System.Reflection.Emit.TypeBuilder DefineType(bool inSignedModulePreferably, string name, System.Reflection.TypeAttributes flags) { }
        [System.ObsoleteAttribute("Exposes a component that is intended for internal use only.")]
        public System.Type GetFromCache(Castle.DynamicProxy.Generators.CacheKey key) { }
        public static byte[] GetKeyPair() { }
        public System.Reflection.Emit.ModuleBuilder ObtainDynamicModule(bool isStrongNamed) { }
        public System.Reflection.Emit.ModuleBuilder ObtainDynamicModuleWithStrongName() { }
        public System.Reflection.Emit.ModuleBuilder ObtainDynamicModuleWithWeakName() { }
        [System.ObsoleteAttribute("Exposes a component that is intended for internal use only.")]
        public void RegisterInCache(Castle.DynamicProxy.Generators.CacheKey key, System.Type type) { }
    }
    public class ProxyGenerationException : System.Exception
    {
        public ProxyGenerationException(string message) { }
        public ProxyGenerationException(string message, System.Exception innerException) { }
    }
    public class ProxyGenerationOptions
    {
        public static readonly Castle.DynamicProxy.ProxyGenerationOptions Default;
        public ProxyGenerationOptions(Castle.DynamicProxy.IProxyGenerationHook hook) { }
        public ProxyGenerationOptions() { }
        public System.Collections.Generic.IList<Castle.DynamicProxy.CustomAttributeInfo> AdditionalAttributes { get; }
        public System.Type BaseTypeForInterfaceProxy { get; set; }
        public bool HasMixins { get; }
        public Castle.DynamicProxy.IProxyGenerationHook Hook { get; set; }
        public Castle.DynamicProxy.MixinData MixinData { get; }
        public Castle.DynamicProxy.IInterceptorSelector Selector { get; set; }
        public void AddDelegateMixin(System.Delegate @delegate) { }
        public void AddDelegateTypeMixin(System.Type delegateType) { }
        public void AddMixinInstance(object instance) { }
        public override bool Equals(object obj) { }
        public override int GetHashCode() { }
        public void Initialize() { }
        public object[] MixinsAsArray() { }
    }
    [System.CLSCompliantAttribute(true)]
    public class ProxyGenerator : Castle.DynamicProxy.IProxyGenerator
    {
        public ProxyGenerator(Castle.DynamicProxy.IProxyBuilder builder) { }
        public ProxyGenerator() { }
        public ProxyGenerator(bool disableSignedModule) { }
        public Castle.Core.Logging.ILogger Logger { get; set; }
        public Castle.DynamicProxy.IProxyBuilder ProxyBuilder { get; }
        protected System.Collections.Generic.List<object> BuildArgumentListForClassProxy(Castle.DynamicProxy.ProxyGenerationOptions options, Castle.DynamicProxy.IInterceptor[] interceptors) { }
        protected System.Collections.Generic.List<object> BuildArgumentListForClassProxyWithTarget(object target, Castle.DynamicProxy.ProxyGenerationOptions options, Castle.DynamicProxy.IInterceptor[] interceptors) { }
        protected void CheckNotGenericTypeDefinition(System.Type type, string argumentName) { }
        protected void CheckNotGenericTypeDefinitions(System.Collections.Generic.IEnumerable<System.Type> types, string argumentName) { }
        public TClass CreateClassProxy<TClass>(params Castle.DynamicProxy.IInterceptor[] interceptors)
            where TClass :  class { }
        public TClass CreateClassProxy<TClass>(Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors)
            where TClass :  class { }
        public object CreateClassProxy(System.Type classToProxy, System.Type[] additionalInterfacesToProxy, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public object CreateClassProxy(System.Type classToProxy, Castle.DynamicProxy.ProxyGenerationOptions options, object[] constructorArguments, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public object CreateClassProxy(System.Type classToProxy, object[] constructorArguments, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public object CreateClassProxy(System.Type classToProxy, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public object CreateClassProxy(System.Type classToProxy, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public object CreateClassProxy(System.Type classToProxy, System.Type[] additionalInterfacesToProxy, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public virtual object CreateClassProxy(System.Type classToProxy, System.Type[] additionalInterfacesToProxy, Castle.DynamicProxy.ProxyGenerationOptions options, object[] constructorArguments, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        protected object CreateClassProxyInstance(System.Type proxyType, System.Collections.Generic.List<object> proxyArguments, System.Type classToProxy, object[] constructorArguments) { }
        protected System.Type CreateClassProxyType(System.Type classToProxy, System.Type[] additionalInterfacesToProxy, Castle.DynamicProxy.ProxyGenerationOptions options) { }
        protected System.Type CreateClassProxyTypeWithTarget(System.Type classToProxy, System.Type[] additionalInterfacesToProxy, Castle.DynamicProxy.ProxyGenerationOptions options) { }
        public TClass CreateClassProxyWithTarget<TClass>(TClass target, params Castle.DynamicProxy.IInterceptor[] interceptors)
            where TClass :  class { }
        public TClass CreateClassProxyWithTarget<TClass>(TClass target, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors)
            where TClass :  class { }
        public object CreateClassProxyWithTarget(System.Type classToProxy, System.Type[] additionalInterfacesToProxy, object target, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public object CreateClassProxyWithTarget(System.Type classToProxy, object target, Castle.DynamicProxy.ProxyGenerationOptions options, object[] constructorArguments, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public object CreateClassProxyWithTarget(System.Type classToProxy, object target, object[] constructorArguments, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public object CreateClassProxyWithTarget(System.Type classToProxy, object target, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public object CreateClassProxyWithTarget(System.Type classToProxy, object target, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public object CreateClassProxyWithTarget(System.Type classToProxy, System.Type[] additionalInterfacesToProxy, object target, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public virtual object CreateClassProxyWithTarget(System.Type classToProxy, System.Type[] additionalInterfacesToProxy, object target, Castle.DynamicProxy.ProxyGenerationOptions options, object[] constructorArguments, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        protected System.Type CreateInterfaceProxyTypeWithTarget(System.Type interfaceToProxy, System.Type[] additionalInterfacesToProxy, System.Type targetType, Castle.DynamicProxy.ProxyGenerationOptions options) { }
        protected System.Type CreateInterfaceProxyTypeWithTargetInterface(System.Type interfaceToProxy, System.Type[] additionalInterfacesToProxy, Castle.DynamicProxy.ProxyGenerationOptions options) { }
        protected System.Type CreateInterfaceProxyTypeWithoutTarget(System.Type interfaceToProxy, System.Type[] additionalInterfacesToProxy, Castle.DynamicProxy.ProxyGenerationOptions options) { }
        public TInterface CreateInterfaceProxyWithTarget<TInterface>(TInterface target, params Castle.DynamicProxy.IInterceptor[] interceptors)
            where TInterface :  class { }
        public TInterface CreateInterfaceProxyWithTarget<TInterface>(TInterface target, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors)
            where TInterface :  class { }
        public object CreateInterfaceProxyWithTarget(System.Type interfaceToProxy, object target, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public object CreateInterfaceProxyWithTarget(System.Type interfaceToProxy, object target, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public object CreateInterfaceProxyWithTarget(System.Type interfaceToProxy, System.Type[] additionalInterfacesToProxy, object target, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public virtual object CreateInterfaceProxyWithTarget(System.Type interfaceToProxy, System.Type[] additionalInterfacesToProxy, object target, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public object CreateInterfaceProxyWithTargetInterface(System.Type interfaceToProxy, object target, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public TInterface CreateInterfaceProxyWithTargetInterface<TInterface>(TInterface target, params Castle.DynamicProxy.IInterceptor[] interceptors)
            where TInterface :  class { }
        public TInterface CreateInterfaceProxyWithTargetInterface<TInterface>(TInterface target, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors)
            where TInterface :  class { }
        public object CreateInterfaceProxyWithTargetInterface(System.Type interfaceToProxy, System.Type[] additionalInterfacesToProxy, object target, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public object CreateInterfaceProxyWithTargetInterface(System.Type interfaceToProxy, object target, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public virtual object CreateInterfaceProxyWithTargetInterface(System.Type interfaceToProxy, System.Type[] additionalInterfacesToProxy, object target, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public TInterface CreateInterfaceProxyWithoutTarget<TInterface>(Castle.DynamicProxy.IInterceptor interceptor)
            where TInterface :  class { }
        public TInterface CreateInterfaceProxyWithoutTarget<TInterface>(params Castle.DynamicProxy.IInterceptor[] interceptors)
            where TInterface :  class { }
        public TInterface CreateInterfaceProxyWithoutTarget<TInterface>(Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors)
            where TInterface :  class { }
        public object CreateInterfaceProxyWithoutTarget(System.Type interfaceToProxy, Castle.DynamicProxy.IInterceptor interceptor) { }
        public object CreateInterfaceProxyWithoutTarget(System.Type interfaceToProxy, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public object CreateInterfaceProxyWithoutTarget(System.Type interfaceToProxy, System.Type[] additionalInterfacesToProxy, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public object CreateInterfaceProxyWithoutTarget(System.Type interfaceToProxy, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        public virtual object CreateInterfaceProxyWithoutTarget(System.Type interfaceToProxy, System.Type[] additionalInterfacesToProxy, Castle.DynamicProxy.ProxyGenerationOptions options, params Castle.DynamicProxy.IInterceptor[] interceptors) { }
        protected System.Collections.Generic.List<object> GetConstructorArguments(object target, Castle.DynamicProxy.IInterceptor[] interceptors, Castle.DynamicProxy.ProxyGenerationOptions options) { }
    }
    public class static ProxyUtil
    {
        public static TDelegate CreateDelegateToMixin<TDelegate>(object proxy) { }
        public static System.Delegate CreateDelegateToMixin(object proxy, System.Type delegateType) { }
        public static object GetUnproxiedInstance(object instance) { }
        public static System.Type GetUnproxiedType(object instance) { }
        public static bool IsAccessible(System.Reflection.MethodBase method) { }
        public static bool IsAccessible(System.Reflection.MethodBase method, out string message) { }
        public static bool IsAccessible(System.Type type) { }
        public static bool IsProxy(object instance) { }
        public static bool IsProxyType(System.Type type) { }
    }
    public class StandardInterceptor : Castle.DynamicProxy.IInterceptor
    {
        public StandardInterceptor() { }
        public void Intercept(Castle.DynamicProxy.IInvocation invocation) { }
        protected virtual void PerformProceed(Castle.DynamicProxy.IInvocation invocation) { }
        protected virtual void PostProceed(Castle.DynamicProxy.IInvocation invocation) { }
        protected virtual void PreProceed(Castle.DynamicProxy.IInvocation invocation) { }
    }
}
namespace Castle.DynamicProxy.Contributors
{
    public class ClassMembersCollector : Castle.DynamicProxy.Contributors.MembersCollector
    {
        public ClassMembersCollector(System.Type targetType) { }
        protected override Castle.DynamicProxy.Generators.MetaMethod GetMethodToGenerate(System.Reflection.MethodInfo method, Castle.DynamicProxy.IProxyGenerationHook hook, bool isStandalone) { }
    }
    public class ClassProxyInstanceContributor : Castle.DynamicProxy.Contributors.ProxyInstanceContributor
    {
        public ClassProxyInstanceContributor(System.Type targetType, System.Collections.Generic.IList<System.Reflection.MethodInfo> methodsToSkip, System.Type[] interfaces, string typeId) { }
        public override void Generate(Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options) { }
        protected override Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference GetTargetReference(Castle.DynamicProxy.Generators.Emitters.ClassEmitter emitter) { }
    }
    public class ClassProxyTargetContributor : Castle.DynamicProxy.Contributors.CompositeTypeContributor
    {
        public ClassProxyTargetContributor(System.Type targetType, System.Collections.Generic.IList<System.Reflection.MethodInfo> methodsToSkip, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
        protected override System.Collections.Generic.IEnumerable<Castle.DynamicProxy.Contributors.MembersCollector> CollectElementsToProxyInternal(Castle.DynamicProxy.IProxyGenerationHook hook) { }
        protected override Castle.DynamicProxy.Generators.MethodGenerator GetMethodGenerator(Castle.DynamicProxy.Generators.MetaMethod method, Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options, Castle.DynamicProxy.Contributors.OverrideMethodDelegate overrideMethod) { }
    }
    public class ClassProxyWithTargetTargetContributor : Castle.DynamicProxy.Contributors.CompositeTypeContributor
    {
        public ClassProxyWithTargetTargetContributor(System.Type targetType, System.Collections.Generic.IList<System.Reflection.MethodInfo> methodsToSkip, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
        protected override System.Collections.Generic.IEnumerable<Castle.DynamicProxy.Contributors.MembersCollector> CollectElementsToProxyInternal(Castle.DynamicProxy.IProxyGenerationHook hook) { }
        protected override Castle.DynamicProxy.Generators.MethodGenerator GetMethodGenerator(Castle.DynamicProxy.Generators.MetaMethod method, Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options, Castle.DynamicProxy.Contributors.OverrideMethodDelegate overrideMethod) { }
    }
    public abstract class CompositeTypeContributor : Castle.DynamicProxy.Contributors.ITypeContributor
    {
        protected readonly System.Collections.Generic.ICollection<System.Type> interfaces;
        protected readonly Castle.DynamicProxy.Generators.INamingScope namingScope;
        protected CompositeTypeContributor(Castle.DynamicProxy.Generators.INamingScope namingScope) { }
        public Castle.Core.Logging.ILogger Logger { get; set; }
        public void AddInterfaceToProxy(System.Type @interface) { }
        public void CollectElementsToProxy(Castle.DynamicProxy.IProxyGenerationHook hook, Castle.DynamicProxy.Generators.MetaType model) { }
        protected abstract System.Collections.Generic.IEnumerable<Castle.DynamicProxy.Contributors.MembersCollector> CollectElementsToProxyInternal(Castle.DynamicProxy.IProxyGenerationHook hook);
        public virtual void Generate(Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options) { }
        protected abstract Castle.DynamicProxy.Generators.MethodGenerator GetMethodGenerator(Castle.DynamicProxy.Generators.MetaMethod method, Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options, Castle.DynamicProxy.Contributors.OverrideMethodDelegate overrideMethod);
    }
    public class DelegateProxyTargetContributor : Castle.DynamicProxy.Contributors.CompositeTypeContributor
    {
        public DelegateProxyTargetContributor(System.Type targetType, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
        protected override System.Collections.Generic.IEnumerable<Castle.DynamicProxy.Contributors.MembersCollector> CollectElementsToProxyInternal(Castle.DynamicProxy.IProxyGenerationHook hook) { }
        protected override Castle.DynamicProxy.Generators.MethodGenerator GetMethodGenerator(Castle.DynamicProxy.Generators.MetaMethod method, Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options, Castle.DynamicProxy.Contributors.OverrideMethodDelegate overrideMethod) { }
    }
    public class DelegateTypeGenerator : Castle.DynamicProxy.Generators.IGenerator<Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter>
    {
        public DelegateTypeGenerator(Castle.DynamicProxy.Generators.MetaMethod method, System.Type targetType) { }
        public Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter Generate(Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
    }
    public class FieldReferenceComparer : System.Collections.Generic.IComparer<System.Type>
    {
        public FieldReferenceComparer() { }
        public int Compare(System.Type x, System.Type y) { }
    }
    public class ForwardingMethodGenerator : Castle.DynamicProxy.Generators.MethodGenerator
    {
        public ForwardingMethodGenerator(Castle.DynamicProxy.Generators.MetaMethod method, Castle.DynamicProxy.Contributors.OverrideMethodDelegate overrideMethod, Castle.DynamicProxy.Contributors.GetTargetReferenceDelegate getTargetReference) { }
        protected override Castle.DynamicProxy.Generators.Emitters.MethodEmitter BuildProxiedMethodBody(Castle.DynamicProxy.Generators.Emitters.MethodEmitter emitter, Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
    }
    public delegate Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression GetTargetExpressionDelegate(Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, System.Reflection.MethodInfo method);
    public delegate Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference GetTargetReferenceDelegate(Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, System.Reflection.MethodInfo method);
    public interface ITypeContributor
    {
        void CollectElementsToProxy(Castle.DynamicProxy.IProxyGenerationHook hook, Castle.DynamicProxy.Generators.MetaType model);
        void Generate(Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options);
    }
    public class InterfaceMembersCollector : Castle.DynamicProxy.Contributors.MembersCollector
    {
        public InterfaceMembersCollector(System.Type @interface) { }
        protected override Castle.DynamicProxy.Generators.MetaMethod GetMethodToGenerate(System.Reflection.MethodInfo method, Castle.DynamicProxy.IProxyGenerationHook hook, bool isStandalone) { }
    }
    public class InterfaceMembersOnClassCollector : Castle.DynamicProxy.Contributors.MembersCollector
    {
        public InterfaceMembersOnClassCollector(System.Type type, bool onlyProxyVirtual, System.Reflection.InterfaceMapping map) { }
        protected override Castle.DynamicProxy.Generators.MetaMethod GetMethodToGenerate(System.Reflection.MethodInfo method, Castle.DynamicProxy.IProxyGenerationHook hook, bool isStandalone) { }
    }
    public class InterfaceProxyInstanceContributor : Castle.DynamicProxy.Contributors.ProxyInstanceContributor
    {
        public InterfaceProxyInstanceContributor(System.Type targetType, string proxyGeneratorId, System.Type[] interfaces) { }
        protected override Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference GetTargetReference(Castle.DynamicProxy.Generators.Emitters.ClassEmitter emitter) { }
    }
    public class InterfaceProxyTargetContributor : Castle.DynamicProxy.Contributors.CompositeTypeContributor
    {
        public InterfaceProxyTargetContributor(System.Type proxyTargetType, bool canChangeTarget, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
        protected override System.Collections.Generic.IEnumerable<Castle.DynamicProxy.Contributors.MembersCollector> CollectElementsToProxyInternal(Castle.DynamicProxy.IProxyGenerationHook hook) { }
        protected virtual Castle.DynamicProxy.Contributors.MembersCollector GetCollectorForInterface(System.Type @interface) { }
        protected override Castle.DynamicProxy.Generators.MethodGenerator GetMethodGenerator(Castle.DynamicProxy.Generators.MetaMethod method, Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options, Castle.DynamicProxy.Contributors.OverrideMethodDelegate overrideMethod) { }
    }
    public class InterfaceProxyWithOptionalTargetContributor : Castle.DynamicProxy.Contributors.InterfaceProxyWithoutTargetContributor
    {
        public InterfaceProxyWithOptionalTargetContributor(Castle.DynamicProxy.Generators.INamingScope namingScope, Castle.DynamicProxy.Contributors.GetTargetExpressionDelegate getTarget, Castle.DynamicProxy.Contributors.GetTargetReferenceDelegate getTargetReference) { }
        protected override Castle.DynamicProxy.Generators.MethodGenerator GetMethodGenerator(Castle.DynamicProxy.Generators.MetaMethod method, Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options, Castle.DynamicProxy.Contributors.OverrideMethodDelegate overrideMethod) { }
    }
    public class InterfaceProxyWithTargetInterfaceTargetContributor : Castle.DynamicProxy.Contributors.InterfaceProxyTargetContributor
    {
        public InterfaceProxyWithTargetInterfaceTargetContributor(System.Type proxyTargetType, bool allowChangeTarget, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
        protected override Castle.DynamicProxy.Contributors.MembersCollector GetCollectorForInterface(System.Type @interface) { }
    }
    public class InterfaceProxyWithoutTargetContributor : Castle.DynamicProxy.Contributors.CompositeTypeContributor
    {
        protected bool canChangeTarget;
        public InterfaceProxyWithoutTargetContributor(Castle.DynamicProxy.Generators.INamingScope namingScope, Castle.DynamicProxy.Contributors.GetTargetExpressionDelegate getTarget) { }
        protected override System.Collections.Generic.IEnumerable<Castle.DynamicProxy.Contributors.MembersCollector> CollectElementsToProxyInternal(Castle.DynamicProxy.IProxyGenerationHook hook) { }
        protected override Castle.DynamicProxy.Generators.MethodGenerator GetMethodGenerator(Castle.DynamicProxy.Generators.MetaMethod method, Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options, Castle.DynamicProxy.Contributors.OverrideMethodDelegate overrideMethod) { }
    }
    public class InvocationWithDelegateContributor : Castle.DynamicProxy.Generators.IInvocationCreationContributor
    {
        public InvocationWithDelegateContributor(System.Type delegateType, System.Type targetType, Castle.DynamicProxy.Generators.MetaMethod method, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
        public Castle.DynamicProxy.Generators.Emitters.ConstructorEmitter CreateConstructor(Castle.DynamicProxy.Generators.Emitters.SimpleAST.ArgumentReference[] baseCtorArguments, Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter invocation) { }
        public System.Reflection.MethodInfo GetCallbackMethod() { }
        public Castle.DynamicProxy.Generators.Emitters.SimpleAST.MethodInvocationExpression GetCallbackMethodInvocation(Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter invocation, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression[] args, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference targetField, Castle.DynamicProxy.Generators.Emitters.MethodEmitter invokeMethodOnTarget) { }
        public Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression[] GetConstructorInvocationArguments(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression[] arguments, Castle.DynamicProxy.Generators.Emitters.ClassEmitter proxy) { }
    }
    public class InvocationWithGenericDelegateContributor : Castle.DynamicProxy.Generators.IInvocationCreationContributor
    {
        public InvocationWithGenericDelegateContributor(System.Type delegateType, Castle.DynamicProxy.Generators.MetaMethod method, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference targetReference) { }
        public Castle.DynamicProxy.Generators.Emitters.ConstructorEmitter CreateConstructor(Castle.DynamicProxy.Generators.Emitters.SimpleAST.ArgumentReference[] baseCtorArguments, Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter invocation) { }
        public System.Reflection.MethodInfo GetCallbackMethod() { }
        public Castle.DynamicProxy.Generators.Emitters.SimpleAST.MethodInvocationExpression GetCallbackMethodInvocation(Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter invocation, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression[] args, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference targetField, Castle.DynamicProxy.Generators.Emitters.MethodEmitter invokeMethodOnTarget) { }
        public Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression[] GetConstructorInvocationArguments(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression[] arguments, Castle.DynamicProxy.Generators.Emitters.ClassEmitter proxy) { }
    }
    public abstract class MembersCollector
    {
        protected readonly System.Type type;
        protected MembersCollector(System.Type type) { }
        public System.Collections.Generic.IEnumerable<Castle.DynamicProxy.Generators.MetaEvent> Events { get; }
        public Castle.Core.Logging.ILogger Logger { get; set; }
        public System.Collections.Generic.IEnumerable<Castle.DynamicProxy.Generators.MetaMethod> Methods { get; }
        public System.Collections.Generic.IEnumerable<Castle.DynamicProxy.Generators.MetaProperty> Properties { get; }
        protected bool AcceptMethod(System.Reflection.MethodInfo method, bool onlyVirtuals, Castle.DynamicProxy.IProxyGenerationHook hook) { }
        public virtual void CollectMembersToProxy(Castle.DynamicProxy.IProxyGenerationHook hook) { }
        protected abstract Castle.DynamicProxy.Generators.MetaMethod GetMethodToGenerate(System.Reflection.MethodInfo method, Castle.DynamicProxy.IProxyGenerationHook hook, bool isStandalone);
    }
    public class MinimialisticMethodGenerator : Castle.DynamicProxy.Generators.MethodGenerator
    {
        public MinimialisticMethodGenerator(Castle.DynamicProxy.Generators.MetaMethod method, Castle.DynamicProxy.Contributors.OverrideMethodDelegate overrideMethod) { }
        protected override Castle.DynamicProxy.Generators.Emitters.MethodEmitter BuildProxiedMethodBody(Castle.DynamicProxy.Generators.Emitters.MethodEmitter emitter, Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
    }
    public class MixinContributor : Castle.DynamicProxy.Contributors.CompositeTypeContributor
    {
        public MixinContributor(Castle.DynamicProxy.Generators.INamingScope namingScope, bool canChangeTarget) { }
        public System.Collections.Generic.IEnumerable<Castle.DynamicProxy.Generators.Emitters.SimpleAST.FieldReference> Fields { get; }
        public void AddEmptyInterface(System.Type @interface) { }
        protected override System.Collections.Generic.IEnumerable<Castle.DynamicProxy.Contributors.MembersCollector> CollectElementsToProxyInternal(Castle.DynamicProxy.IProxyGenerationHook hook) { }
        public override void Generate(Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options) { }
        protected override Castle.DynamicProxy.Generators.MethodGenerator GetMethodGenerator(Castle.DynamicProxy.Generators.MetaMethod method, Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options, Castle.DynamicProxy.Contributors.OverrideMethodDelegate overrideMethod) { }
    }
    public class OptionallyForwardingMethodGenerator : Castle.DynamicProxy.Generators.MethodGenerator
    {
        public OptionallyForwardingMethodGenerator(Castle.DynamicProxy.Generators.MetaMethod method, Castle.DynamicProxy.Contributors.OverrideMethodDelegate overrideMethod, Castle.DynamicProxy.Contributors.GetTargetReferenceDelegate getTargetReference) { }
        protected override Castle.DynamicProxy.Generators.Emitters.MethodEmitter BuildProxiedMethodBody(Castle.DynamicProxy.Generators.Emitters.MethodEmitter emitter, Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
    }
    public delegate Castle.DynamicProxy.Generators.Emitters.MethodEmitter OverrideMethodDelegate(string name, System.Reflection.MethodAttributes attributes, System.Reflection.MethodInfo methodToOverride);
    public abstract class ProxyInstanceContributor : Castle.DynamicProxy.Contributors.ITypeContributor
    {
        protected readonly System.Type targetType;
        protected ProxyInstanceContributor(System.Type targetType, System.Type[] interfaces, string proxyTypeId) { }
        public void CollectElementsToProxy(Castle.DynamicProxy.IProxyGenerationHook hook, Castle.DynamicProxy.Generators.MetaType model) { }
        public virtual void Generate(Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options) { }
        protected abstract Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference GetTargetReference(Castle.DynamicProxy.Generators.Emitters.ClassEmitter emitter);
        protected void ImplementProxyTargetAccessor(Castle.DynamicProxy.Generators.Emitters.ClassEmitter emitter, Castle.DynamicProxy.Generators.Emitters.SimpleAST.FieldReference interceptorsField) { }
    }
    public class WrappedClassMembersCollector : Castle.DynamicProxy.Contributors.ClassMembersCollector
    {
        public WrappedClassMembersCollector(System.Type type) { }
        public override void CollectMembersToProxy(Castle.DynamicProxy.IProxyGenerationHook hook) { }
        protected override Castle.DynamicProxy.Generators.MetaMethod GetMethodToGenerate(System.Reflection.MethodInfo method, Castle.DynamicProxy.IProxyGenerationHook hook, bool isStandalone) { }
        protected bool IsGeneratedByTheCompiler(System.Reflection.FieldInfo field) { }
        protected virtual bool IsOKToBeOnProxy(System.Reflection.FieldInfo field) { }
    }
}
namespace Castle.DynamicProxy.Generators
{
    public class static AttributesToAvoidReplicating
    {
        public static void Add(System.Type attribute) { }
        public static void Add<T>() { }
        public static bool Contains(System.Type attribute) { }
    }
    public abstract class BaseProxyGenerator
    {
        protected readonly System.Type targetType;
        protected BaseProxyGenerator(Castle.DynamicProxy.ModuleScope scope, System.Type targetType) { }
        public Castle.Core.Logging.ILogger Logger { get; set; }
        protected Castle.DynamicProxy.ProxyGenerationOptions ProxyGenerationOptions { get; set; }
        protected Castle.DynamicProxy.ModuleScope Scope { get; }
        protected void AddMapping(System.Type @interface, Castle.DynamicProxy.Contributors.ITypeContributor implementer, System.Collections.Generic.IDictionary<System.Type, Castle.DynamicProxy.Contributors.ITypeContributor> mapping) { }
        protected void AddMappingNoCheck(System.Type @interface, Castle.DynamicProxy.Contributors.ITypeContributor implementer, System.Collections.Generic.IDictionary<System.Type, Castle.DynamicProxy.Contributors.ITypeContributor> mapping) { }
        [System.ObsoleteAttribute("Exposes a component that is intended for internal use only.")]
        protected void AddToCache(Castle.DynamicProxy.Generators.CacheKey key, System.Type type) { }
        protected virtual Castle.DynamicProxy.Generators.Emitters.ClassEmitter BuildClassEmitter(string typeName, System.Type parentType, System.Collections.Generic.IEnumerable<System.Type> interfaces) { }
        protected void CheckNotGenericTypeDefinition(System.Type type, string argumentName) { }
        protected void CheckNotGenericTypeDefinitions(System.Collections.Generic.IEnumerable<System.Type> types, string argumentName) { }
        protected void CompleteInitCacheMethod(Castle.DynamicProxy.Generators.Emitters.CodeBuilders.ConstructorCodeBuilder constCodeBuilder) { }
        protected virtual void CreateFields(Castle.DynamicProxy.Generators.Emitters.ClassEmitter emitter) { }
        protected void CreateInterceptorsField(Castle.DynamicProxy.Generators.Emitters.ClassEmitter emitter) { }
        protected Castle.DynamicProxy.Generators.Emitters.SimpleAST.FieldReference CreateOptionsField(Castle.DynamicProxy.Generators.Emitters.ClassEmitter emitter) { }
        protected void CreateSelectorField(Castle.DynamicProxy.Generators.Emitters.ClassEmitter emitter) { }
        protected virtual void CreateTypeAttributes(Castle.DynamicProxy.Generators.Emitters.ClassEmitter emitter) { }
        protected void EnsureOptionsOverrideEqualsAndGetHashCode(Castle.DynamicProxy.ProxyGenerationOptions options) { }
        protected void GenerateConstructor(Castle.DynamicProxy.Generators.Emitters.ClassEmitter emitter, System.Reflection.ConstructorInfo baseConstructor, params Castle.DynamicProxy.Generators.Emitters.SimpleAST.FieldReference[] fields) { }
        protected void GenerateConstructors(Castle.DynamicProxy.Generators.Emitters.ClassEmitter emitter, System.Type baseType, params Castle.DynamicProxy.Generators.Emitters.SimpleAST.FieldReference[] fields) { }
        protected void GenerateParameterlessConstructor(Castle.DynamicProxy.Generators.Emitters.ClassEmitter emitter, System.Type baseClass, Castle.DynamicProxy.Generators.Emitters.SimpleAST.FieldReference interceptorField) { }
        protected Castle.DynamicProxy.Generators.Emitters.ConstructorEmitter GenerateStaticConstructor(Castle.DynamicProxy.Generators.Emitters.ClassEmitter emitter) { }
        [System.ObsoleteAttribute("Exposes a component that is intended for internal use only.")]
        protected System.Type GetFromCache(Castle.DynamicProxy.Generators.CacheKey key) { }
        protected void HandleExplicitlyPassedProxyTargetAccessor(System.Collections.Generic.ICollection<System.Type> targetInterfaces, System.Collections.Generic.ICollection<System.Type> additionalInterfaces) { }
        protected void InitializeStaticFields(System.Type builtType) { }
        [System.ObsoleteAttribute("Exposes a component that is intended for internal use only.")]
        protected System.Type ObtainProxyType(Castle.DynamicProxy.Generators.CacheKey cacheKey, System.Func<string, Castle.DynamicProxy.Generators.INamingScope, System.Type> factory) { }
    }
    [System.ObsoleteAttribute("Intended for internal use only.")]
    public class CacheKey
    {
        public CacheKey(System.Reflection.MemberInfo target, System.Type type, System.Type[] interfaces, Castle.DynamicProxy.ProxyGenerationOptions options) { }
        public CacheKey(System.Type target, System.Type[] interfaces, Castle.DynamicProxy.ProxyGenerationOptions options) { }
        public override bool Equals(object obj) { }
        public override int GetHashCode() { }
    }
    public class ClassProxyGenerator : Castle.DynamicProxy.Generators.BaseProxyGenerator
    {
        public ClassProxyGenerator(Castle.DynamicProxy.ModuleScope scope, System.Type targetType) { }
        public System.Type GenerateCode(System.Type[] interfaces, Castle.DynamicProxy.ProxyGenerationOptions options) { }
        protected virtual System.Type GenerateType(string name, System.Type[] interfaces, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
        protected virtual System.Collections.Generic.IEnumerable<System.Type> GetTypeImplementerMapping(System.Type[] interfaces, out System.Collections.Generic.IEnumerable<Castle.DynamicProxy.Contributors.ITypeContributor> contributors, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
    }
    public class ClassProxyWithTargetGenerator : Castle.DynamicProxy.Generators.BaseProxyGenerator
    {
        public ClassProxyWithTargetGenerator(Castle.DynamicProxy.ModuleScope scope, System.Type classToProxy, System.Type[] additionalInterfacesToProxy, Castle.DynamicProxy.ProxyGenerationOptions options) { }
        public System.Type GetGeneratedType() { }
        protected virtual System.Collections.Generic.IEnumerable<System.Type> GetTypeImplementerMapping(out System.Collections.Generic.IEnumerable<Castle.DynamicProxy.Contributors.ITypeContributor> contributors, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
    }
    public class CompositionInvocationTypeGenerator : Castle.DynamicProxy.Generators.InvocationTypeGenerator
    {
        public static readonly System.Type BaseType;
        public CompositionInvocationTypeGenerator(System.Type target, Castle.DynamicProxy.Generators.MetaMethod method, System.Reflection.MethodInfo callback, bool canChangeTarget, Castle.DynamicProxy.Generators.IInvocationCreationContributor contributor) { }
        protected override Castle.DynamicProxy.Generators.Emitters.SimpleAST.ArgumentReference[] GetBaseCtorArguments(System.Type targetFieldType, Castle.DynamicProxy.ProxyGenerationOptions proxyGenerationOptions, out System.Reflection.ConstructorInfo baseConstructor) { }
        protected override System.Type GetBaseType() { }
        protected override Castle.DynamicProxy.Generators.Emitters.SimpleAST.FieldReference GetTargetReference() { }
        protected override void ImplementInvokeMethodOnTarget(Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter invocation, System.Reflection.ParameterInfo[] parameters, Castle.DynamicProxy.Generators.Emitters.MethodEmitter invokeMethodOnTarget, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference targetField) { }
    }
    public class DelegateMembersCollector : Castle.DynamicProxy.Contributors.MembersCollector
    {
        public DelegateMembersCollector(System.Type type) { }
        protected override Castle.DynamicProxy.Generators.MetaMethod GetMethodToGenerate(System.Reflection.MethodInfo method, Castle.DynamicProxy.IProxyGenerationHook hook, bool isStandalone) { }
    }
    public class DelegateProxyGenerationHook : Castle.DynamicProxy.IProxyGenerationHook
    {
        public DelegateProxyGenerationHook() { }
        public override bool Equals(object obj) { }
        public override int GetHashCode() { }
        public void MethodsInspected() { }
        public void NonProxyableMemberNotification(System.Type type, System.Reflection.MemberInfo memberInfo) { }
        public bool ShouldInterceptMethod(System.Type type, System.Reflection.MethodInfo methodInfo) { }
    }
    public class DelegateProxyGenerator : Castle.DynamicProxy.Generators.BaseProxyGenerator
    {
        public DelegateProxyGenerator(Castle.DynamicProxy.ModuleScope scope, System.Type delegateType) { }
        public System.Type GetProxyType() { }
        protected virtual System.Collections.Generic.IEnumerable<System.Type> GetTypeImplementerMapping(out System.Collections.Generic.IEnumerable<Castle.DynamicProxy.Contributors.ITypeContributor> contributors, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
    }
    public class GeneratorException : System.Exception
    {
        public GeneratorException(string message) { }
        public GeneratorException(string message, System.Exception innerException) { }
    }
    public class static GeneratorUtil
    {
        public static void CopyOutAndRefParameters(Castle.DynamicProxy.Generators.Emitters.SimpleAST.TypeReference[] dereferencedArguments, Castle.DynamicProxy.Generators.Emitters.SimpleAST.LocalReference invocation, System.Reflection.MethodInfo method, Castle.DynamicProxy.Generators.Emitters.MethodEmitter emitter) { }
    }
    public interface IGenerator<T>
    {
        T Generate(Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options, Castle.DynamicProxy.Generators.INamingScope namingScope);
    }
    public interface IInvocationCreationContributor
    {
        Castle.DynamicProxy.Generators.Emitters.ConstructorEmitter CreateConstructor(Castle.DynamicProxy.Generators.Emitters.SimpleAST.ArgumentReference[] baseCtorArguments, Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter invocation);
        System.Reflection.MethodInfo GetCallbackMethod();
        Castle.DynamicProxy.Generators.Emitters.SimpleAST.MethodInvocationExpression GetCallbackMethodInvocation(Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter invocation, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression[] args, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference targetField, Castle.DynamicProxy.Generators.Emitters.MethodEmitter invokeMethodOnTarget);
        Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression[] GetConstructorInvocationArguments(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression[] arguments, Castle.DynamicProxy.Generators.Emitters.ClassEmitter proxy);
    }
    public interface INamingScope
    {
        Castle.DynamicProxy.Generators.INamingScope ParentScope { get; }
        string GetUniqueName(string suggestedName);
        Castle.DynamicProxy.Generators.INamingScope SafeSubScope();
    }
    public class InheritanceInvocationTypeGenerator : Castle.DynamicProxy.Generators.InvocationTypeGenerator
    {
        public static readonly System.Type BaseType;
        public InheritanceInvocationTypeGenerator(System.Type targetType, Castle.DynamicProxy.Generators.MetaMethod method, System.Reflection.MethodInfo callback, Castle.DynamicProxy.Generators.IInvocationCreationContributor contributor) { }
        protected override Castle.DynamicProxy.Generators.Emitters.SimpleAST.ArgumentReference[] GetBaseCtorArguments(System.Type targetFieldType, Castle.DynamicProxy.ProxyGenerationOptions proxyGenerationOptions, out System.Reflection.ConstructorInfo baseConstructor) { }
        protected override System.Type GetBaseType() { }
        protected override Castle.DynamicProxy.Generators.Emitters.SimpleAST.FieldReference GetTargetReference() { }
    }
    public class InterfaceProxyWithTargetGenerator : Castle.DynamicProxy.Generators.BaseProxyGenerator
    {
        protected Castle.DynamicProxy.Generators.Emitters.SimpleAST.FieldReference targetField;
        public InterfaceProxyWithTargetGenerator(Castle.DynamicProxy.ModuleScope scope, System.Type @interface) { }
        protected virtual bool AllowChangeTarget { get; }
        protected virtual string GeneratorType { get; }
        protected virtual Castle.DynamicProxy.Contributors.ITypeContributor AddMappingForTargetType(System.Collections.Generic.IDictionary<System.Type, Castle.DynamicProxy.Contributors.ITypeContributor> typeImplementerMapping, System.Type proxyTargetType, System.Collections.Generic.ICollection<System.Type> targetInterfaces, System.Collections.Generic.ICollection<System.Type> additionalInterfaces, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
        public System.Type GenerateCode(System.Type proxyTargetType, System.Type[] interfaces, Castle.DynamicProxy.ProxyGenerationOptions options) { }
        protected virtual System.Type GenerateType(string typeName, System.Type proxyTargetType, System.Type[] interfaces, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
        protected virtual Castle.DynamicProxy.Contributors.InterfaceProxyWithoutTargetContributor GetContributorForAdditionalInterfaces(Castle.DynamicProxy.Generators.INamingScope namingScope) { }
        protected virtual System.Collections.Generic.IEnumerable<System.Type> GetTypeImplementerMapping(System.Type[] interfaces, System.Type proxyTargetType, out System.Collections.Generic.IEnumerable<Castle.DynamicProxy.Contributors.ITypeContributor> contributors, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
        protected virtual System.Type Init(string typeName, out Castle.DynamicProxy.Generators.Emitters.ClassEmitter emitter, System.Type proxyTargetType, out Castle.DynamicProxy.Generators.Emitters.SimpleAST.FieldReference interceptorsField, System.Collections.Generic.IEnumerable<System.Type> interfaces) { }
    }
    public class InterfaceProxyWithTargetInterfaceGenerator : Castle.DynamicProxy.Generators.InterfaceProxyWithTargetGenerator
    {
        public InterfaceProxyWithTargetInterfaceGenerator(Castle.DynamicProxy.ModuleScope scope, System.Type @interface) { }
        protected override bool AllowChangeTarget { get; }
        protected override string GeneratorType { get; }
        protected override Castle.DynamicProxy.Contributors.ITypeContributor AddMappingForTargetType(System.Collections.Generic.IDictionary<System.Type, Castle.DynamicProxy.Contributors.ITypeContributor> typeImplementerMapping, System.Type proxyTargetType, System.Collections.Generic.ICollection<System.Type> targetInterfaces, System.Collections.Generic.ICollection<System.Type> additionalInterfaces, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
        protected override Castle.DynamicProxy.Contributors.InterfaceProxyWithoutTargetContributor GetContributorForAdditionalInterfaces(Castle.DynamicProxy.Generators.INamingScope namingScope) { }
    }
    public class InterfaceProxyWithoutTargetGenerator : Castle.DynamicProxy.Generators.InterfaceProxyWithTargetGenerator
    {
        public InterfaceProxyWithoutTargetGenerator(Castle.DynamicProxy.ModuleScope scope, System.Type @interface) { }
        protected override string GeneratorType { get; }
        protected override Castle.DynamicProxy.Contributors.ITypeContributor AddMappingForTargetType(System.Collections.Generic.IDictionary<System.Type, Castle.DynamicProxy.Contributors.ITypeContributor> interfaceTypeImplementerMapping, System.Type proxyTargetType, System.Collections.Generic.ICollection<System.Type> targetInterfaces, System.Collections.Generic.ICollection<System.Type> additionalInterfaces, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
        protected override System.Type GenerateType(string typeName, System.Type proxyTargetType, System.Type[] interfaces, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
    }
    public abstract class InvocationTypeGenerator : Castle.DynamicProxy.Generators.IGenerator<Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter>
    {
        protected readonly Castle.DynamicProxy.Generators.MetaMethod method;
        protected readonly System.Type targetType;
        protected InvocationTypeGenerator(System.Type targetType, Castle.DynamicProxy.Generators.MetaMethod method, System.Reflection.MethodInfo callback, bool canChangeTarget, Castle.DynamicProxy.Generators.IInvocationCreationContributor contributor) { }
        public Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter Generate(Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
        protected abstract Castle.DynamicProxy.Generators.Emitters.SimpleAST.ArgumentReference[] GetBaseCtorArguments(System.Type targetFieldType, Castle.DynamicProxy.ProxyGenerationOptions proxyGenerationOptions, out System.Reflection.ConstructorInfo baseConstructor);
        protected abstract System.Type GetBaseType();
        protected virtual Castle.DynamicProxy.Generators.Emitters.SimpleAST.MethodInvocationExpression GetCallbackMethodInvocation(Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter invocation, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression[] args, System.Reflection.MethodInfo callbackMethod, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference targetField, Castle.DynamicProxy.Generators.Emitters.MethodEmitter invokeMethodOnTarget) { }
        protected abstract Castle.DynamicProxy.Generators.Emitters.SimpleAST.FieldReference GetTargetReference();
        protected virtual void ImplementInvokeMethodOnTarget(Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter invocation, System.Reflection.ParameterInfo[] parameters, Castle.DynamicProxy.Generators.Emitters.MethodEmitter invokeMethodOnTarget, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference targetField) { }
    }
    public class MetaEvent : Castle.DynamicProxy.Generators.MetaTypeElement, System.IEquatable<Castle.DynamicProxy.Generators.MetaEvent>
    {
        public MetaEvent(string name, System.Type declaringType, System.Type eventDelegateType, Castle.DynamicProxy.Generators.MetaMethod adder, Castle.DynamicProxy.Generators.MetaMethod remover, System.Reflection.EventAttributes attributes) { }
        public Castle.DynamicProxy.Generators.MetaMethod Adder { get; }
        public System.Reflection.EventAttributes Attributes { get; }
        public Castle.DynamicProxy.Generators.Emitters.EventEmitter Emitter { get; }
        public Castle.DynamicProxy.Generators.MetaMethod Remover { get; }
        public void BuildEventEmitter(Castle.DynamicProxy.Generators.Emitters.ClassEmitter classEmitter) { }
        public override bool Equals(object obj) { }
        public bool Equals(Castle.DynamicProxy.Generators.MetaEvent other) { }
        public override int GetHashCode() { }
    }
    [System.Diagnostics.DebuggerDisplayAttribute("{Method}")]
    public class MetaMethod : Castle.DynamicProxy.Generators.MetaTypeElement, System.IEquatable<Castle.DynamicProxy.Generators.MetaMethod>
    {
        public MetaMethod(System.Reflection.MethodInfo method, System.Reflection.MethodInfo methodOnTarget, bool standalone, bool proxyable, bool hasTarget) { }
        public System.Reflection.MethodAttributes Attributes { get; }
        public bool HasTarget { get; }
        public System.Reflection.MethodInfo Method { get; }
        public System.Reflection.MethodInfo MethodOnTarget { get; }
        public string Name { get; }
        public bool Proxyable { get; }
        public bool Standalone { get; }
        public bool Equals(Castle.DynamicProxy.Generators.MetaMethod other) { }
    }
    public class MetaProperty : Castle.DynamicProxy.Generators.MetaTypeElement, System.IEquatable<Castle.DynamicProxy.Generators.MetaProperty>
    {
        public MetaProperty(string name, System.Type propertyType, System.Type declaringType, Castle.DynamicProxy.Generators.MetaMethod getter, Castle.DynamicProxy.Generators.MetaMethod setter, System.Collections.Generic.IEnumerable<System.Reflection.Emit.CustomAttributeBuilder> customAttributes, System.Type[] arguments) { }
        public System.Type[] Arguments { get; }
        public bool CanRead { get; }
        public bool CanWrite { get; }
        public Castle.DynamicProxy.Generators.Emitters.PropertyEmitter Emitter { get; }
        public System.Reflection.MethodInfo GetMethod { get; }
        public Castle.DynamicProxy.Generators.MetaMethod Getter { get; }
        public System.Reflection.MethodInfo SetMethod { get; }
        public Castle.DynamicProxy.Generators.MetaMethod Setter { get; }
        public void BuildPropertyEmitter(Castle.DynamicProxy.Generators.Emitters.ClassEmitter classEmitter) { }
        public override bool Equals(object obj) { }
        public bool Equals(Castle.DynamicProxy.Generators.MetaProperty other) { }
        public override int GetHashCode() { }
    }
    public class MetaType
    {
        public MetaType() { }
        public System.Collections.Generic.IEnumerable<Castle.DynamicProxy.Generators.MetaEvent> Events { get; }
        public System.Collections.Generic.IEnumerable<Castle.DynamicProxy.Generators.MetaMethod> Methods { get; }
        public System.Collections.Generic.IEnumerable<Castle.DynamicProxy.Generators.MetaProperty> Properties { get; }
        public void AddEvent(Castle.DynamicProxy.Generators.MetaEvent @event) { }
        public void AddMethod(Castle.DynamicProxy.Generators.MetaMethod method) { }
        public void AddProperty(Castle.DynamicProxy.Generators.MetaProperty property) { }
    }
    public abstract class MetaTypeElement
    {
        protected readonly System.Type sourceType;
        protected MetaTypeElement(System.Type sourceType) { }
    }
    public class MethodFinder
    {
        public MethodFinder() { }
        public static System.Reflection.MethodInfo[] GetAllInstanceMethods(System.Type type, System.Reflection.BindingFlags flags) { }
    }
    public abstract class MethodGenerator : Castle.DynamicProxy.Generators.IGenerator<Castle.DynamicProxy.Generators.Emitters.MethodEmitter>
    {
        protected MethodGenerator(Castle.DynamicProxy.Generators.MetaMethod method, Castle.DynamicProxy.Contributors.OverrideMethodDelegate overrideMethod) { }
        protected System.Reflection.MethodInfo MethodOnTarget { get; }
        protected System.Reflection.MethodInfo MethodToOverride { get; }
        protected abstract Castle.DynamicProxy.Generators.Emitters.MethodEmitter BuildProxiedMethodBody(Castle.DynamicProxy.Generators.Emitters.MethodEmitter emitter, Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options, Castle.DynamicProxy.Generators.INamingScope namingScope);
        public Castle.DynamicProxy.Generators.Emitters.MethodEmitter Generate(Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
    }
    public class MethodSignatureComparer : System.Collections.Generic.IEqualityComparer<System.Reflection.MethodInfo>
    {
        public static readonly Castle.DynamicProxy.Generators.MethodSignatureComparer Instance;
        public MethodSignatureComparer() { }
        public bool EqualGenericParameters(System.Reflection.MethodInfo x, System.Reflection.MethodInfo y) { }
        public bool EqualParameters(System.Reflection.MethodInfo x, System.Reflection.MethodInfo y) { }
        public bool EqualSignatureTypes(System.Type x, System.Type y) { }
        public bool Equals(System.Reflection.MethodInfo x, System.Reflection.MethodInfo y) { }
        public int GetHashCode(System.Reflection.MethodInfo obj) { }
    }
    public class MethodWithInvocationGenerator : Castle.DynamicProxy.Generators.MethodGenerator
    {
        public MethodWithInvocationGenerator(Castle.DynamicProxy.Generators.MetaMethod method, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference interceptors, System.Type invocation, Castle.DynamicProxy.Contributors.GetTargetExpressionDelegate getTargetExpression, Castle.DynamicProxy.Contributors.OverrideMethodDelegate createMethod, Castle.DynamicProxy.Generators.IInvocationCreationContributor contributor) { }
        public MethodWithInvocationGenerator(Castle.DynamicProxy.Generators.MetaMethod method, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference interceptors, System.Type invocation, Castle.DynamicProxy.Contributors.GetTargetExpressionDelegate getTargetExpression, Castle.DynamicProxy.Contributors.GetTargetExpressionDelegate getTargetTypeExpression, Castle.DynamicProxy.Contributors.OverrideMethodDelegate createMethod, Castle.DynamicProxy.Generators.IInvocationCreationContributor contributor) { }
        protected Castle.DynamicProxy.Generators.Emitters.SimpleAST.FieldReference BuildMethodInterceptorsField(Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, System.Reflection.MethodInfo method, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
        protected override Castle.DynamicProxy.Generators.Emitters.MethodEmitter BuildProxiedMethodBody(Castle.DynamicProxy.Generators.Emitters.MethodEmitter emitter, Castle.DynamicProxy.Generators.Emitters.ClassEmitter @class, Castle.DynamicProxy.ProxyGenerationOptions options, Castle.DynamicProxy.Generators.INamingScope namingScope) { }
    }
    public class NamingScope : Castle.DynamicProxy.Generators.INamingScope
    {
        public NamingScope() { }
        public Castle.DynamicProxy.Generators.INamingScope ParentScope { get; }
        public string GetUniqueName(string suggestedName) { }
        public Castle.DynamicProxy.Generators.INamingScope SafeSubScope() { }
    }
    public class TypeElementCollection<TElement> : System.Collections.Generic.ICollection<TElement>, System.Collections.Generic.IEnumerable<TElement>, System.Collections.IEnumerable
        where TElement : Castle.DynamicProxy.Generators.MetaTypeElement, System.IEquatable<TElement>
    {
        public TypeElementCollection() { }
        public int Count { get; }
        public void Add(TElement item) { }
        public bool Contains(TElement item) { }
        public System.Collections.Generic.IEnumerator<TElement> GetEnumerator() { }
    }
}
namespace Castle.DynamicProxy.Generators.Emitters
{
    public abstract class AbstractTypeEmitter
    {
        protected AbstractTypeEmitter(System.Reflection.Emit.TypeBuilder typeBuilder) { }
        public System.Type BaseType { get; }
        public Castle.DynamicProxy.Generators.Emitters.TypeConstructorEmitter ClassConstructor { get; }
        public Castle.DynamicProxy.Generators.Emitters.ConstructorCollection Constructors { get; }
        public System.Reflection.Emit.GenericTypeParameterBuilder[] GenericTypeParams { get; }
        public Castle.DynamicProxy.Generators.Emitters.NestedClassCollection Nested { get; }
        public System.Reflection.Emit.TypeBuilder TypeBuilder { get; }
        public void AddCustomAttributes(Castle.DynamicProxy.ProxyGenerationOptions proxyGenerationOptions) { }
        public virtual System.Type BuildType() { }
        public void CopyGenericParametersFromMethod(System.Reflection.MethodInfo methodToCopyGenericsFrom) { }
        public Castle.DynamicProxy.Generators.Emitters.ConstructorEmitter CreateConstructor(params Castle.DynamicProxy.Generators.Emitters.SimpleAST.ArgumentReference[] arguments) { }
        public void CreateDefaultConstructor() { }
        public Castle.DynamicProxy.Generators.Emitters.EventEmitter CreateEvent(string name, System.Reflection.EventAttributes atts, System.Type type) { }
        public Castle.DynamicProxy.Generators.Emitters.SimpleAST.FieldReference CreateField(string name, System.Type fieldType) { }
        public Castle.DynamicProxy.Generators.Emitters.SimpleAST.FieldReference CreateField(string name, System.Type fieldType, bool serializable) { }
        public Castle.DynamicProxy.Generators.Emitters.SimpleAST.FieldReference CreateField(string name, System.Type fieldType, System.Reflection.FieldAttributes atts) { }
        public Castle.DynamicProxy.Generators.Emitters.MethodEmitter CreateMethod(string name, System.Reflection.MethodAttributes attrs, System.Type returnType, params System.Type[] argumentTypes) { }
        public Castle.DynamicProxy.Generators.Emitters.MethodEmitter CreateMethod(string name, System.Type returnType, params System.Type[] parameterTypes) { }
        public Castle.DynamicProxy.Generators.Emitters.MethodEmitter CreateMethod(string name, System.Reflection.MethodInfo methodToUseAsATemplate) { }
        public Castle.DynamicProxy.Generators.Emitters.MethodEmitter CreateMethod(string name, System.Reflection.MethodAttributes attributes, System.Reflection.MethodInfo methodToUseAsATemplate) { }
        public Castle.DynamicProxy.Generators.Emitters.PropertyEmitter CreateProperty(string name, System.Reflection.PropertyAttributes attributes, System.Type propertyType, System.Type[] arguments) { }
        public Castle.DynamicProxy.Generators.Emitters.SimpleAST.FieldReference CreateStaticField(string name, System.Type fieldType) { }
        public Castle.DynamicProxy.Generators.Emitters.SimpleAST.FieldReference CreateStaticField(string name, System.Type fieldType, System.Reflection.FieldAttributes atts) { }
        protected System.Type CreateType(System.Reflection.Emit.TypeBuilder type) { }
        public Castle.DynamicProxy.Generators.Emitters.ConstructorEmitter CreateTypeConstructor() { }
        public void DefineCustomAttribute(System.Reflection.Emit.CustomAttributeBuilder attribute) { }
        public void DefineCustomAttribute<TAttribute>(object[] constructorArguments)
            where TAttribute : System.Attribute { }
        public void DefineCustomAttribute<TAttribute>()
            where TAttribute : System.Attribute, new () { }
        public void DefineCustomAttributeFor<TAttribute>(Castle.DynamicProxy.Generators.Emitters.SimpleAST.FieldReference field)
            where TAttribute : System.Attribute, new () { }
        protected virtual void EnsureBuildersAreInAValidState() { }
        public System.Collections.Generic.IEnumerable<Castle.DynamicProxy.Generators.Emitters.SimpleAST.FieldReference> GetAllFields() { }
        public Castle.DynamicProxy.Generators.Emitters.SimpleAST.FieldReference GetField(string name) { }
        public System.Type GetGenericArgument(string genericArgumentName) { }
        public System.Type[] GetGenericArgumentsFor(System.Type genericType) { }
        public System.Type[] GetGenericArgumentsFor(System.Reflection.MethodInfo genericMethod) { }
        public void SetGenericTypeParameters(System.Reflection.Emit.GenericTypeParameterBuilder[] genericTypeParameterBuilders) { }
    }
    public abstract class ArgumentsUtil
    {
        protected ArgumentsUtil() { }
        public static Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression[] ConvertArgumentReferenceToExpression(Castle.DynamicProxy.Generators.Emitters.SimpleAST.ArgumentReference[] args) { }
        public static Castle.DynamicProxy.Generators.Emitters.SimpleAST.ArgumentReference[] ConvertToArgumentReference(System.Type[] args) { }
        public static Castle.DynamicProxy.Generators.Emitters.SimpleAST.ArgumentReference[] ConvertToArgumentReference(System.Reflection.ParameterInfo[] args) { }
        public static Castle.DynamicProxy.Generators.Emitters.SimpleAST.ReferenceExpression[] ConvertToArgumentReferenceExpression(System.Reflection.ParameterInfo[] args) { }
        public static void EmitLoadOwnerAndReference(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference reference, System.Reflection.Emit.ILGenerator il) { }
        public static System.Type[] GetTypes(System.Reflection.ParameterInfo[] parameters) { }
        public static System.Type[] InitializeAndConvert(Castle.DynamicProxy.Generators.Emitters.SimpleAST.ArgumentReference[] args) { }
        public static void InitializeArgumentsByPosition(Castle.DynamicProxy.Generators.Emitters.SimpleAST.ArgumentReference[] args, bool isStatic) { }
        [System.ObsoleteAttribute()]
        public static bool IsAnyByRef(System.Reflection.ParameterInfo[] parameters) { }
    }
    public class ClassEmitter : Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter
    {
        public ClassEmitter(Castle.DynamicProxy.ModuleScope modulescope, string name, System.Type baseType, System.Collections.Generic.IEnumerable<System.Type> interfaces) { }
        public ClassEmitter(Castle.DynamicProxy.ModuleScope modulescope, string name, System.Type baseType, System.Collections.Generic.IEnumerable<System.Type> interfaces, System.Reflection.TypeAttributes flags) { }
        public ClassEmitter(Castle.DynamicProxy.ModuleScope modulescope, string name, System.Type baseType, System.Collections.Generic.IEnumerable<System.Type> interfaces, System.Reflection.TypeAttributes flags, bool forceUnsigned) { }
        public ClassEmitter(System.Reflection.Emit.TypeBuilder typeBuilder) { }
        public Castle.DynamicProxy.ModuleScope ModuleScope { get; }
        protected virtual System.Collections.Generic.IEnumerable<System.Type> InitializeGenericArgumentsFromBases(ref System.Type baseType, System.Collections.Generic.IEnumerable<System.Type> interfaces) { }
    }
    public class ConstructorCollection : System.Collections.ObjectModel.Collection<Castle.DynamicProxy.Generators.Emitters.ConstructorEmitter>
    {
        public ConstructorCollection() { }
    }
    public class ConstructorEmitter : Castle.DynamicProxy.Generators.Emitters.IMemberEmitter
    {
        protected ConstructorEmitter(Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter maintype, System.Reflection.Emit.ConstructorBuilder builder) { }
        public virtual Castle.DynamicProxy.Generators.Emitters.CodeBuilders.ConstructorCodeBuilder CodeBuilder { get; }
        public System.Reflection.Emit.ConstructorBuilder ConstructorBuilder { get; }
        public System.Reflection.MemberInfo Member { get; }
        public System.Type ReturnType { get; }
        public virtual void EnsureValidCodeBlock() { }
        public virtual void Generate() { }
    }
    public class EventCollection : System.Collections.ObjectModel.Collection<Castle.DynamicProxy.Generators.Emitters.EventEmitter>
    {
        public EventCollection() { }
    }
    public class EventEmitter : Castle.DynamicProxy.Generators.Emitters.IMemberEmitter
    {
        public EventEmitter(Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter typeEmitter, string name, System.Reflection.EventAttributes attributes, System.Type type) { }
        public System.Reflection.MemberInfo Member { get; }
        public System.Type ReturnType { get; }
        public Castle.DynamicProxy.Generators.Emitters.MethodEmitter CreateAddMethod(string addMethodName, System.Reflection.MethodAttributes attributes, System.Reflection.MethodInfo methodToOverride) { }
        public Castle.DynamicProxy.Generators.Emitters.MethodEmitter CreateRemoveMethod(string removeMethodName, System.Reflection.MethodAttributes attributes, System.Reflection.MethodInfo methodToOverride) { }
        public void EnsureValidCodeBlock() { }
        public void Generate() { }
    }
    public interface IMemberEmitter
    {
        System.Reflection.MemberInfo Member { get; }
        System.Type ReturnType { get; }
        void EnsureValidCodeBlock();
        void Generate();
    }
    public sealed class LdcOpCodesDictionary : System.Collections.Generic.Dictionary<System.Type, System.Reflection.Emit.OpCode>
    {
        public static System.Reflection.Emit.OpCode EmptyOpCode { get; }
        public static Castle.DynamicProxy.Generators.Emitters.LdcOpCodesDictionary Instance { get; }
        public System.Reflection.Emit.OpCode this[System.Type type] { get; }
    }
    public sealed class LdindOpCodesDictionary : System.Collections.Generic.Dictionary<System.Type, System.Reflection.Emit.OpCode>
    {
        public static System.Reflection.Emit.OpCode EmptyOpCode { get; }
        public static Castle.DynamicProxy.Generators.Emitters.LdindOpCodesDictionary Instance { get; }
        public System.Reflection.Emit.OpCode this[System.Type type] { get; }
    }
    public class MethodCollection : System.Collections.ObjectModel.Collection<Castle.DynamicProxy.Generators.Emitters.MethodEmitter>
    {
        public MethodCollection() { }
    }
    [System.Diagnostics.DebuggerDisplayAttribute("{builder.Name}")]
    public class MethodEmitter : Castle.DynamicProxy.Generators.Emitters.IMemberEmitter
    {
        protected MethodEmitter(System.Reflection.Emit.MethodBuilder builder) { }
        public Castle.DynamicProxy.Generators.Emitters.SimpleAST.ArgumentReference[] Arguments { get; }
        public virtual Castle.DynamicProxy.Generators.Emitters.CodeBuilders.MethodCodeBuilder CodeBuilder { get; }
        public System.Reflection.Emit.GenericTypeParameterBuilder[] GenericTypeParams { get; }
        public System.Reflection.MemberInfo Member { get; }
        public System.Reflection.Emit.MethodBuilder MethodBuilder { get; }
        public System.Type ReturnType { get; }
        public void DefineCustomAttribute(System.Reflection.Emit.CustomAttributeBuilder attribute) { }
        public virtual void EnsureValidCodeBlock() { }
        public virtual void Generate() { }
        public void SetParameters(System.Type[] paramTypes) { }
    }
    public class NestedClassCollection : System.Collections.ObjectModel.Collection<Castle.DynamicProxy.Generators.Emitters.NestedClassEmitter>
    {
        public NestedClassCollection() { }
    }
    public class NestedClassEmitter : Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter
    {
        public NestedClassEmitter(Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter maintype, string name, System.Type baseType, System.Type[] interfaces) { }
        public NestedClassEmitter(Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter maintype, string name, System.Reflection.TypeAttributes attributes, System.Type baseType, System.Type[] interfaces) { }
        public NestedClassEmitter(Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter maintype, System.Reflection.Emit.TypeBuilder typeBuilder) { }
    }
    public class PropertiesCollection : System.Collections.ObjectModel.Collection<Castle.DynamicProxy.Generators.Emitters.PropertyEmitter>
    {
        public PropertiesCollection() { }
    }
    public class PropertyEmitter : Castle.DynamicProxy.Generators.Emitters.IMemberEmitter
    {
        public PropertyEmitter(Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter parentTypeEmitter, string name, System.Reflection.PropertyAttributes attributes, System.Type propertyType, System.Type[] arguments) { }
        public System.Reflection.MemberInfo Member { get; }
        public System.Type ReturnType { get; }
        public Castle.DynamicProxy.Generators.Emitters.MethodEmitter CreateGetMethod(string name, System.Reflection.MethodAttributes attrs, System.Reflection.MethodInfo methodToOverride, params System.Type[] parameters) { }
        public Castle.DynamicProxy.Generators.Emitters.MethodEmitter CreateGetMethod(string name, System.Reflection.MethodAttributes attributes, System.Reflection.MethodInfo methodToOverride) { }
        public Castle.DynamicProxy.Generators.Emitters.MethodEmitter CreateSetMethod(string name, System.Reflection.MethodAttributes attrs, System.Reflection.MethodInfo methodToOverride, params System.Type[] parameters) { }
        public Castle.DynamicProxy.Generators.Emitters.MethodEmitter CreateSetMethod(string name, System.Reflection.MethodAttributes attributes, System.Reflection.MethodInfo methodToOverride) { }
        public void DefineCustomAttribute(System.Reflection.Emit.CustomAttributeBuilder attribute) { }
        public void EnsureValidCodeBlock() { }
        public void Generate() { }
    }
    public sealed class StindOpCodesDictionary : System.Collections.Generic.Dictionary<System.Type, System.Reflection.Emit.OpCode>
    {
        public static System.Reflection.Emit.OpCode EmptyOpCode { get; }
        public static Castle.DynamicProxy.Generators.Emitters.StindOpCodesDictionary Instance { get; }
        public System.Reflection.Emit.OpCode this[System.Type type] { get; }
    }
    public class static StrongNameUtil
    {
        public static bool CanStrongNameAssembly { get; set; }
        public static bool IsAnyTypeFromUnsignedAssembly(System.Collections.Generic.IEnumerable<System.Type> types) { }
        public static bool IsAnyTypeFromUnsignedAssembly(System.Type baseType, System.Collections.Generic.IEnumerable<System.Type> interfaces) { }
        public static bool IsAssemblySigned(this System.Reflection.Assembly assembly) { }
    }
    public class TypeConstructorEmitter : Castle.DynamicProxy.Generators.Emitters.ConstructorEmitter
    {
        public override void EnsureValidCodeBlock() { }
    }
}
namespace Castle.DynamicProxy.Generators.Emitters.CodeBuilders
{
    public abstract class AbstractCodeBuilder
    {
        protected AbstractCodeBuilder(System.Reflection.Emit.ILGenerator generator) { }
        public System.Reflection.Emit.ILGenerator Generator { get; }
        public Castle.DynamicProxy.Generators.Emitters.CodeBuilders.AbstractCodeBuilder AddExpression(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression expression) { }
        public Castle.DynamicProxy.Generators.Emitters.CodeBuilders.AbstractCodeBuilder AddStatement(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Statement stmt) { }
        public Castle.DynamicProxy.Generators.Emitters.SimpleAST.LocalReference DeclareLocal(System.Type type) { }
        public void SetNonEmpty() { }
    }
    public class ConstructorCodeBuilder : Castle.DynamicProxy.Generators.Emitters.CodeBuilders.AbstractCodeBuilder
    {
        public ConstructorCodeBuilder(System.Type baseType, System.Reflection.Emit.ILGenerator generator) { }
        public void InvokeBaseConstructor() { }
        public void InvokeBaseConstructor(System.Reflection.ConstructorInfo constructor) { }
        public void InvokeBaseConstructor(System.Reflection.ConstructorInfo constructor, params Castle.DynamicProxy.Generators.Emitters.SimpleAST.ArgumentReference[] arguments) { }
    }
    public class MethodCodeBuilder : Castle.DynamicProxy.Generators.Emitters.CodeBuilders.AbstractCodeBuilder
    {
        public MethodCodeBuilder(System.Reflection.Emit.ILGenerator generator) { }
    }
}
namespace Castle.DynamicProxy.Generators.Emitters.SimpleAST
{
    public class AddressOfReferenceExpression : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression
    {
        public AddressOfReferenceExpression(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference reference) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    [System.Diagnostics.DebuggerDisplayAttribute("argument {Type}")]
    public class ArgumentReference : Castle.DynamicProxy.Generators.Emitters.SimpleAST.TypeReference
    {
        public ArgumentReference(System.Type argumentType) { }
        public ArgumentReference(System.Type argumentType, int position) { }
        public override void LoadAddressOfReference(System.Reflection.Emit.ILGenerator gen) { }
        public override void LoadReference(System.Reflection.Emit.ILGenerator gen) { }
        public override void StoreReference(System.Reflection.Emit.ILGenerator gen) { }
    }
    [System.Diagnostics.DebuggerDisplayAttribute("{reference} as {type}")]
    public class AsTypeReference : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference
    {
        public AsTypeReference(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference reference, System.Type type) { }
        public override void LoadAddressOfReference(System.Reflection.Emit.ILGenerator gen) { }
        public override void LoadReference(System.Reflection.Emit.ILGenerator gen) { }
        public override void StoreReference(System.Reflection.Emit.ILGenerator gen) { }
    }
    public class AssignArgumentStatement : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Statement
    {
        public AssignArgumentStatement(Castle.DynamicProxy.Generators.Emitters.SimpleAST.ArgumentReference argument, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression expression) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    public class AssignArrayStatement : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Statement
    {
        public AssignArrayStatement(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference targetArray, int targetPosition, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression value) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator il) { }
    }
    public class AssignStatement : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Statement
    {
        public AssignStatement(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference target, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression expression) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    public class BindDelegateExpression : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression
    {
        public BindDelegateExpression(System.Type @delegate, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression owner, System.Reflection.MethodInfo methodToBindTo, System.Reflection.Emit.GenericTypeParameterBuilder[] genericTypeParams) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    [System.Diagnostics.DebuggerDisplayAttribute("&{localReference}")]
    public class ByRefReference : Castle.DynamicProxy.Generators.Emitters.SimpleAST.TypeReference
    {
        public ByRefReference(Castle.DynamicProxy.Generators.Emitters.SimpleAST.LocalReference localReference) { }
        public override void LoadAddressOfReference(System.Reflection.Emit.ILGenerator gen) { }
        public override void LoadReference(System.Reflection.Emit.ILGenerator gen) { }
        public override void StoreReference(System.Reflection.Emit.ILGenerator gen) { }
    }
    [System.Diagnostics.DebuggerDisplayAttribute("{value}")]
    public class ConstReference : Castle.DynamicProxy.Generators.Emitters.SimpleAST.TypeReference
    {
        public ConstReference(object value) { }
        public override void Generate(System.Reflection.Emit.ILGenerator gen) { }
        public override void LoadAddressOfReference(System.Reflection.Emit.ILGenerator gen) { }
        public override void LoadReference(System.Reflection.Emit.ILGenerator gen) { }
        public override void StoreReference(System.Reflection.Emit.ILGenerator gen) { }
    }
    public class ConstructorInvocationStatement : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Statement
    {
        public ConstructorInvocationStatement(System.Reflection.ConstructorInfo method, params Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression[] args) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    public class ConvertExpression : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression
    {
        public ConvertExpression(System.Type targetType, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression right) { }
        public ConvertExpression(System.Type targetType, System.Type fromType, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression right) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    public class DefaultValueExpression : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression
    {
        public DefaultValueExpression(System.Type type) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    public class EndExceptionBlockStatement : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Statement
    {
        public EndExceptionBlockStatement() { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    public abstract class Expression : Castle.DynamicProxy.Generators.Emitters.SimpleAST.IILEmitter
    {
        protected Expression() { }
        public abstract void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen);
    }
    public class ExpressionStatement : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Statement
    {
        public ExpressionStatement(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression expression) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    [System.Diagnostics.DebuggerDisplayAttribute("{fieldbuilder.Name} ({fieldbuilder.FieldType})")]
    public class FieldReference : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference
    {
        public FieldReference(System.Reflection.FieldInfo field) { }
        public FieldReference(System.Reflection.Emit.FieldBuilder fieldbuilder) { }
        public System.Reflection.Emit.FieldBuilder Fieldbuilder { get; }
        public System.Reflection.FieldInfo Reference { get; }
        public override void LoadAddressOfReference(System.Reflection.Emit.ILGenerator gen) { }
        public override void LoadReference(System.Reflection.Emit.ILGenerator gen) { }
        public override void StoreReference(System.Reflection.Emit.ILGenerator gen) { }
    }
    public class FinallyStatement : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Statement
    {
        public FinallyStatement() { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    public interface IILEmitter
    {
        void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen);
    }
    public class IfNullExpression : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression
    {
        public IfNullExpression(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference reference, Castle.DynamicProxy.Generators.Emitters.SimpleAST.IILEmitter ifNull, Castle.DynamicProxy.Generators.Emitters.SimpleAST.IILEmitter ifNotNull = null) { }
        public IfNullExpression(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression expression, Castle.DynamicProxy.Generators.Emitters.SimpleAST.IILEmitter ifNull, Castle.DynamicProxy.Generators.Emitters.SimpleAST.IILEmitter ifNotNull = null) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    [System.Diagnostics.DebuggerDisplayAttribute("&{OwnerReference}")]
    public class IndirectReference : Castle.DynamicProxy.Generators.Emitters.SimpleAST.TypeReference
    {
        public IndirectReference(Castle.DynamicProxy.Generators.Emitters.SimpleAST.TypeReference byRefReference) { }
        public override void LoadAddressOfReference(System.Reflection.Emit.ILGenerator gen) { }
        public override void LoadReference(System.Reflection.Emit.ILGenerator gen) { }
        public override void StoreReference(System.Reflection.Emit.ILGenerator gen) { }
        public static Castle.DynamicProxy.Generators.Emitters.SimpleAST.TypeReference WrapIfByRef(Castle.DynamicProxy.Generators.Emitters.SimpleAST.TypeReference reference) { }
        public static Castle.DynamicProxy.Generators.Emitters.SimpleAST.TypeReference[] WrapIfByRef(Castle.DynamicProxy.Generators.Emitters.SimpleAST.TypeReference[] references) { }
    }
    public class LiteralIntExpression : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression
    {
        public LiteralIntExpression(int value) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    public class LoadArrayElementExpression : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression
    {
        public LoadArrayElementExpression(int index, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference arrayReference, System.Type returnType) { }
        public LoadArrayElementExpression(Castle.DynamicProxy.Generators.Emitters.SimpleAST.ConstReference index, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference arrayReference, System.Type returnType) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    public class LoadRefArrayElementExpression : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression
    {
        public LoadRefArrayElementExpression(int index, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference arrayReference) { }
        public LoadRefArrayElementExpression(Castle.DynamicProxy.Generators.Emitters.SimpleAST.ConstReference index, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference arrayReference) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    [System.Diagnostics.DebuggerDisplayAttribute("local {Type}")]
    public class LocalReference : Castle.DynamicProxy.Generators.Emitters.SimpleAST.TypeReference
    {
        public LocalReference(System.Type type) { }
        public override void Generate(System.Reflection.Emit.ILGenerator gen) { }
        public override void LoadAddressOfReference(System.Reflection.Emit.ILGenerator gen) { }
        public override void LoadReference(System.Reflection.Emit.ILGenerator gen) { }
        public override void StoreReference(System.Reflection.Emit.ILGenerator gen) { }
    }
    public class MethodInvocationExpression : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression
    {
        protected readonly Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression[] args;
        protected readonly System.Reflection.MethodInfo method;
        protected readonly Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference owner;
        public MethodInvocationExpression(System.Reflection.MethodInfo method, params Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression[] args) { }
        public MethodInvocationExpression(Castle.DynamicProxy.Generators.Emitters.MethodEmitter method, params Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression[] args) { }
        public MethodInvocationExpression(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference owner, Castle.DynamicProxy.Generators.Emitters.MethodEmitter method, params Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression[] args) { }
        public MethodInvocationExpression(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference owner, System.Reflection.MethodInfo method, params Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression[] args) { }
        public bool VirtualCall { get; set; }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    public class MethodTokenExpression : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression
    {
        public MethodTokenExpression(System.Reflection.MethodInfo method) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    public class MultiStatementExpression : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression
    {
        public MultiStatementExpression() { }
        public void AddExpression(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression expression) { }
        public void AddStatement(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Statement statement) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    public class NewArrayExpression : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression
    {
        public NewArrayExpression(int size, System.Type arrayType) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    public class NewInstanceExpression : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression
    {
        public NewInstanceExpression(System.Reflection.ConstructorInfo constructor, params Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression[] args) { }
        public NewInstanceExpression(System.Type target, System.Type[] constructor_args, params Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression[] args) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    public class NopStatement : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Statement
    {
        public NopStatement() { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    public class NullCoalescingOperatorExpression : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression
    {
        public NullCoalescingOperatorExpression(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression expression, Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression @default) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    public class NullExpression : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression
    {
        public static readonly Castle.DynamicProxy.Generators.Emitters.SimpleAST.NullExpression Instance;
        protected NullExpression() { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    public abstract class Reference
    {
        protected Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference owner;
        protected Reference() { }
        protected Reference(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference owner) { }
        public Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference OwnerReference { get; set; }
        public virtual void Generate(System.Reflection.Emit.ILGenerator gen) { }
        public abstract void LoadAddressOfReference(System.Reflection.Emit.ILGenerator gen);
        public abstract void LoadReference(System.Reflection.Emit.ILGenerator gen);
        public abstract void StoreReference(System.Reflection.Emit.ILGenerator gen);
        public virtual Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression ToAddressOfExpression() { }
        public virtual Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression ToExpression() { }
    }
    public class ReferenceExpression : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression
    {
        public ReferenceExpression(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference reference) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    public class ReferencesToObjectArrayExpression : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression
    {
        public ReferencesToObjectArrayExpression(params Castle.DynamicProxy.Generators.Emitters.SimpleAST.TypeReference[] args) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    public class ReturnStatement : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Statement
    {
        public ReturnStatement() { }
        public ReturnStatement(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference reference) { }
        public ReturnStatement(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression expression) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    [System.Diagnostics.DebuggerDisplayAttribute("this")]
    public class SelfReference : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference
    {
        public static readonly Castle.DynamicProxy.Generators.Emitters.SimpleAST.SelfReference Self;
        protected SelfReference() { }
        public override void LoadAddressOfReference(System.Reflection.Emit.ILGenerator gen) { }
        public override void LoadReference(System.Reflection.Emit.ILGenerator gen) { }
        public override void StoreReference(System.Reflection.Emit.ILGenerator gen) { }
    }
    public abstract class Statement : Castle.DynamicProxy.Generators.Emitters.SimpleAST.IILEmitter
    {
        protected Statement() { }
        public abstract void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen);
    }
    public class ThrowStatement : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Statement
    {
        public ThrowStatement(System.Type exceptionType, string errorMessage) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    public class TryStatement : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Statement
    {
        public TryStatement() { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
    public abstract class TypeReference : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference
    {
        protected TypeReference(System.Type argumentType) { }
        protected TypeReference(Castle.DynamicProxy.Generators.Emitters.SimpleAST.Reference owner, System.Type type) { }
        public System.Type Type { get; }
    }
    public class TypeTokenExpression : Castle.DynamicProxy.Generators.Emitters.SimpleAST.Expression
    {
        public TypeTokenExpression(System.Type type) { }
        public override void Emit(Castle.DynamicProxy.Generators.Emitters.IMemberEmitter member, System.Reflection.Emit.ILGenerator gen) { }
    }
}
namespace Castle.DynamicProxy.Internal
{
    public class static AttributeUtil
    {
        public static Castle.DynamicProxy.CustomAttributeInfo CreateInfo(System.Reflection.CustomAttributeData attribute) { }
        public static Castle.DynamicProxy.CustomAttributeInfo CreateInfo<TAttribute>()
            where TAttribute : System.Attribute, new () { }
        public static Castle.DynamicProxy.CustomAttributeInfo CreateInfo(System.Type attribute, object[] constructorArguments) { }
        public static System.Collections.Generic.IEnumerable<Castle.DynamicProxy.CustomAttributeInfo> GetNonInheritableAttributes(this System.Reflection.MemberInfo member) { }
        public static System.Collections.Generic.IEnumerable<Castle.DynamicProxy.CustomAttributeInfo> GetNonInheritableAttributes(this System.Reflection.ParameterInfo parameter) { }
    }
    public abstract class CompositionInvocation : Castle.DynamicProxy.AbstractInvocation
    {
        protected object target;
        protected CompositionInvocation(object target, object proxy, Castle.DynamicProxy.IInterceptor[] interceptors, System.Reflection.MethodInfo proxiedMethod, object[] arguments) { }
        public override object InvocationTarget { get; }
        public override System.Reflection.MethodInfo MethodInvocationTarget { get; }
        public override System.Type TargetType { get; }
        protected void EnsureValidProxyTarget(object newTarget) { }
        protected void EnsureValidTarget() { }
    }
    public abstract class InheritanceInvocation : Castle.DynamicProxy.AbstractInvocation
    {
        protected InheritanceInvocation(System.Type targetType, object proxy, Castle.DynamicProxy.IInterceptor[] interceptors, System.Reflection.MethodInfo proxiedMethod, object[] arguments) { }
        public override object InvocationTarget { get; }
        public override System.Reflection.MethodInfo MethodInvocationTarget { get; }
        public override System.Type TargetType { get; }
        protected virtual void InvokeMethodOnTarget() { }
    }
    public class static InternalsUtil
    {
        [System.ObsoleteAttribute("Use ProxyUtil.IsAccessible instead, which performs a more accurate accessibility " +
            "check.")]
        public static bool IsAccessible(this System.Reflection.MethodBase method) { }
        [System.ObsoleteAttribute()]
        public static bool IsInternal(this System.Reflection.MethodBase method) { }
        [System.ObsoleteAttribute()]
        public static bool IsInternalToDynamicProxy(this System.Reflection.Assembly asm) { }
    }
    public class static InvocationHelper
    {
        public static System.Reflection.MethodInfo GetMethodOnObject(object target, System.Reflection.MethodInfo proxiedMethod) { }
        public static System.Reflection.MethodInfo GetMethodOnType(System.Type type, System.Reflection.MethodInfo proxiedMethod) { }
    }
    public class static TypeUtil
    {
        public static System.Type[] AsTypeArray(this System.Reflection.Emit.GenericTypeParameterBuilder[] typeInfos) { }
        public static System.Reflection.FieldInfo[] GetAllFields(this System.Type type) { }
        public static System.Type[] GetAllInterfaces(params System.Type[] types) { }
        public static System.Type[] GetAllInterfaces(this System.Type type) { }
        public static System.Type GetClosedParameterType(this Castle.DynamicProxy.Generators.Emitters.AbstractTypeEmitter type, System.Type parameter) { }
        public static System.Type GetTypeOrNull(object target) { }
        public static bool IsFinalizer(this System.Reflection.MethodInfo methodInfo) { }
        public static bool IsGetType(this System.Reflection.MethodInfo methodInfo) { }
        public static bool IsMemberwiseClone(this System.Reflection.MethodInfo methodInfo) { }
        public static bool IsNullableType(this System.Type type) { }
        public static void SetStaticField(this System.Type type, string fieldName, System.Reflection.BindingFlags additionalFlags, object value) { }
        public static System.Reflection.MemberInfo[] Sort(System.Reflection.MemberInfo[] members) { }
    }
}
namespace Castle.DynamicProxy.Tokens
{
    public class static DelegateMethods
    {
        public static readonly System.Reflection.MethodInfo CreateDelegate;
    }
    public class static InterceptorSelectorMethods
    {
        public static readonly System.Reflection.MethodInfo SelectInterceptors;
    }
    public class static InvocationMethods
    {
        public static readonly System.Reflection.ConstructorInfo CompositionInvocationConstructor;
        public static readonly System.Reflection.MethodInfo CompositionInvocationEnsureValidTarget;
        public static readonly System.Reflection.FieldInfo CompositionInvocationTarget;
        public static readonly System.Reflection.MethodInfo EnsureValidTarget;
        public static readonly System.Reflection.MethodInfo GetArgumentValue;
        public static readonly System.Reflection.MethodInfo GetArguments;
        public static readonly System.Reflection.MethodInfo GetReturnValue;
        public static readonly System.Reflection.ConstructorInfo InheritanceInvocationConstructor;
        public static readonly System.Reflection.ConstructorInfo InheritanceInvocationConstructorWithSelector;
        public static readonly System.Reflection.MethodInfo Proceed;
        public static readonly System.Reflection.FieldInfo ProxyObject;
        public static readonly System.Reflection.MethodInfo SetArgumentValue;
        public static readonly System.Reflection.MethodInfo SetGenericMethodArguments;
        public static readonly System.Reflection.MethodInfo SetReturnValue;
        public static readonly System.Reflection.FieldInfo Target;
        public static readonly System.Reflection.MethodInfo ThrowOnNoTarget;
    }
    public class static MethodBaseMethods
    {
        public static readonly System.Reflection.MethodInfo GetMethodFromHandle;
    }
    public class static TypeMethods
    {
        public static readonly System.Reflection.MethodInfo GetTypeFromHandle;
        public static readonly System.Reflection.MethodInfo StaticGetType;
    }
    public class static TypeUtilMethods
    {
        public static readonly System.Reflection.MethodInfo GetTypeOrNull;
        public static readonly System.Reflection.MethodInfo Sort;
    }
}