
// Sets up assembly level security settings

//[assembly: System.Security.AllowPartiallyTrustedCallers]
#if DOTNET40
[assembly: System.Security.SecurityRules(System.Security.SecurityRuleSet.Level2)]
#endif
