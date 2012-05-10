// Sets up assembly level security settings
#if ! SILVERLIGHT
[assembly: System.Security.AllowPartiallyTrustedCallers]
[assembly: System.Security.SecurityRules(System.Security.SecurityRuleSet.Level2)]
#endif