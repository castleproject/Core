This project exists so we will have a separate assembly which has no Active Record
types in it.
It is here to ensure that these tests will work:
* AssemblyXmlGenerationTestCase.WillUseRegisteredAssembliesToLookForRawMappingXmlEvenIfThereAreNoActiveRecordTypesInThatAssembly
* IntegrationWithNHibernateTestCase.CanIntegrateNHibernateAndActiveRecord

