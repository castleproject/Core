// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Components.DictionaryAdapter.Xml.Tests
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Xml;
	using System.Xml.Schema;
	using System.Xml.Serialization;
	using NUnit.Framework;
	using Castle.Components.DictionaryAdapter.Tests;

	[TestFixture]
	public class XmlAdapterAcceptanceTestCase
	{
		private DictionaryAdapterFactory factory;

		[SetUp]
		public void SetUp()
		{
			factory = new DictionaryAdapterFactory();
		}

		[Test]
		public void Factory_ForXml_CreatesTheAdapter()
		{
			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(null, ref document);
			Assert.IsNotNull(season);
		}

		[Test]
		public void Adapter_OnXml_Will_Preserve_References()
		{
			var line1 = "2922 South Highway 205";
			var city = "Rockwall";
			var state = "TX";
			var zipCode = "75032";

			var xml = string.Format(
				@"<Season xmlns='RISE'>
					 <Address xmlns='Common'>
						<Line1>{0}</Line1>
						<City>{1}</City>
						<State>{2}</State>
						<ZipCode>{3}</ZipCode>
					 </Address>
				  </Season>",
				line1, city, state, zipCode);

			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(xml, ref document);
			Assert.AreSame(season.Location, season.Location);
			Assert.AreEqual(line1, season.Location.Line1);

			var seasonNode = document["Season", "RISE"];
			var addressNode = seasonNode["Address", "Common"];
			var line1Node = addressNode["Line1", "Common"];
			line1Node.InnerText = "1234 Tulip";
			Assert.AreEqual("1234 Tulip", season.Location.Line1);
		}

		[Test]
		public void Adapter_OnXml_CanTargetWithXPath()
		{
			var line1 = "2922 South Highway 205";
			var city = "Rockwall";
			var state = "TX";
			var zipCode = "75032";

			var xml = string.Format(
				@"<Season xmlns='RISE'>
					 <Address xmlns='Common'>
						<Line1>{0}</Line1>
						<City>{1}</City>
						<State>{2}</State>
						<ZipCode>{3}</ZipCode>
					 </Address>
				  </Season>",
				line1, city, state, zipCode);

			XmlDocument document = null;
			var address = CreateXmlAdapter<IAddress>(xml, ref document);
			Assert.AreEqual(line1, address.Line1);
		}

		[Test]
		public void Adapter_OnXml_CanReadProperties()
		{
			var name = "Soccer Adult Winter II 2010";
			var minAge = 30;
			var division = Division.Coed;
			var startsOn = new DateTime(2010, 2, 21);
			var endsOn = new DateTime(2010, 4, 18);
			var line1 = "2922 South Highway 205";
			var city = "Rockwall";
			var state = "TX";
			var zipCode = "75032";
			var team1Name = "Fairly Oddparents";
			var team1Balance = 450.00M;
			var team1Player1FirstName = "Mike";
			var team1Player1LastName = "Etheridge";
			var team1Player2FirstName = "Susan";
			var team1Player2LastName = "Houston";
			var team2Name = "Team Punishment";
			var team2Balance = 655.50M;
			var team2Player1FirstName = "Stephen";
			var team2Player1LastName = "Gray";
			var licenseNo = "1234";
			var tags = new[] { "Soccer", "Skills", "Fun" };

			var xml = string.Format(
				@"<Season xmlns='RISE' xmlns:rise='RISE'>
					 <Name>{0}</Name>
					 <MinimumAge>{1}</MinimumAge>
					 <Division>{2}</Division>
					 <StartsOn>{3}</StartsOn>
					 <EndsOn>{4}</EndsOn>
					 <Address xmlns='Common'>
						<Line1>{5}</Line1>
						<City>{6}</City>
						<State>{7}</State>
						<ZipCode>{8}</ZipCode>
					 </Address>
					 <League>
						<Team Name='{9}'>
						   <AmountDue>{10}</AmountDue>
						   <Roster>
							  <Participant FirstName='{11}' lastName='{12}'>
							  </Participant>
							  <Participant FirstName='{13}' lastName='{14}'>
							  </Participant>
						   </Roster>
						</Team>
						<Team Name='{15}'>
						   <AmountDue>{16}</AmountDue>
						   <Roster>
							  <Participant FirstName='{17}' lastName='{18}'>
							  </Participant>
						   </Roster>
						</Team>
					 </League>
					 <Documentation xmlns=''>
					  <Notes>notes</Notes>
					 </Documentation>
					 <Tag>{19}</Tag>
					 <Tag>{20}</Tag>
					 <Tag>{21}</Tag>
					 <ExtraStuff>
						<LicenseNo>{22}</LicenseNo>
					 </ExtraStuff>
				  </Season>",
				name, minAge, division,
				XmlConvert.ToString(startsOn, XmlDateTimeSerializationMode.Local),
				XmlConvert.ToString(endsOn, XmlDateTimeSerializationMode.Local),
				line1, city, state, zipCode,
				team1Name, XmlConvert.ToString(team1Balance),
				team1Player1FirstName, team1Player1LastName,
				team1Player2FirstName, team1Player2LastName,
				team2Name, XmlConvert.ToString(team2Balance),
				team2Player1FirstName, team2Player1LastName,
				tags[0], tags[1], tags[2],
				licenseNo);

			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(xml, ref document);
			Assert.AreEqual(name, season.Name);
			Assert.AreEqual(minAge, season.MinimumAge);
			Assert.AreEqual(division, season.Division);
			Assert.AreEqual(startsOn.Date, season.StartsOn.Date);
			Assert.AreEqual(endsOn.Date, season.EndsOn.Date);
			Assert.AreEqual(line1, season.Location.Line1);
			Assert.AreEqual(city, season.Location.City);
			Assert.AreEqual(state, season.Location.State);
			Assert.AreEqual(zipCode, season.Location.ZipCode);
			Assert.AreEqual(2, season.Teams.Count);

			var team = season.Teams[0];
			var n = team.Name;

			Assert.AreEqual(team1Name, season.Teams[0].Name);
			Assert.AreEqual(team1Balance, season.Teams[0].Balance);
			Assert.IsNull(season.Teams[0].GamesPlayed);
			Assert.AreEqual(2, season.Teams[0].Players.Count);
			Assert.AreEqual(team1Player1FirstName, season.Teams[0].Players[0].FirstName);
			Assert.AreEqual(team1Player1LastName, season.Teams[0].Players[0].LastName);
			Assert.AreEqual(team1Player2FirstName, season.Teams[0].Players[1].FirstName);
			Assert.AreEqual(team1Player2LastName, season.Teams[0].Players[1].LastName);
			Assert.AreEqual(team2Name, season.Teams[1].Name);
			Assert.AreEqual(team2Balance, season.Teams[1].Balance);
			Assert.AreEqual(1, season.Teams[1].Players.Count);
			Assert.AreEqual(team2Player1FirstName, season.Teams[1].Players[0].FirstName);
			Assert.AreEqual(team2Player1LastName, season.Teams[1].Players[0].LastName);
			Assert.AreEqual(2, season.TeamsArray.Length);
			Assert.AreEqual(team1Name, season.TeamsArray[0].Name);
			Assert.AreEqual(team1Balance, season.TeamsArray[0].Balance);
			Assert.AreEqual(team2Name, season.TeamsArray[1].Name);
			Assert.AreEqual(team2Balance, season.TeamsArray[1].Balance);
			Assert.AreEqual(team1Balance + team2Balance, season.Balance);
			Assert.AreEqual(team1Name, season.FirstTeamName);
			Assert.AreEqual("notes", season.Notes); // TODO: Improve
			Assert.AreEqual(3, season.Tags.Length);
			Assert.Contains(tags[0], season.Tags);
			Assert.Contains(tags[1], season.Tags);
			Assert.Contains(tags[2], season.Tags);
			Assert.IsNotNull(season.ExtraStuff);
			Assert.AreEqual(licenseNo, season.ExtraStuff["LicenseNo", "RISE"].InnerText);
		}

		public interface IFoo
		{
			[XmlAttribute]
			string Name { get; set; }
		}

		[XmlType(Namespace = "urn:fiz.com")]
		[XmlNamespace("urn:fiz.com", "b")]
		public interface IBar
		{
			IFoo Foo { get; set; }
			[XPath("b:Foo/@Name")]
			string FooName { get; }
		}

		[Test]
		public void Adapter_OnXml_CanWriteProperties()
		{
			var name = "Soccer Adult Winter II 2010";
			var minAge = 30;
			var division = Division.Coed;
			var startsOn = new DateTime(2010, 2, 21);
			var endsOn = new DateTime(2010, 4, 18);
			var line1 = "2922 South Highway 205";
			var city = "Rockwall";
			var state = "TX";
			var zipCode = "75032";
			var team1Name = "Fairly Oddparents";
			var team1Balance = 450.00M;
			var team3Name = "Barcelona";
			var team3Balance = 175.15M;
			var notes = "Some Notes";

			var xml = string.Format(
				@"<Season xmlns='RISE' xmlns:rise='RISE'>
					 <Name>Soccer Adult Spring II 2010</Name>
					 <MinimumAge>16</MinimumAge>
					 <Division>Male</Division>
					 <StartsOn>{0}</StartsOn>
					 <EndsOn>{1}</EndsOn>
					 <League>
						<Team name='Hit And Run'>
						   <AmountDue>100.50</AmountDue>
						</Team>
						<Team name='Nemisis'>
						   <AmountDue>250.00</AmountDue>
						</Team>
					 </League>
					 <ExtraStuff>
						<LicenseNo>9999</LicenseNo>
					 </ExtraStuff>
				  </Season>",
				XmlConvert.ToString(new DateTime(2010, 7, 19), XmlDateTimeSerializationMode.Local),
				XmlConvert.ToString(new DateTime(2010, 9, 20), XmlDateTimeSerializationMode.Local));

			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(xml, ref document);
			season.Name = name;
			season.MinimumAge = minAge;
			season.Division = division;
			season.StartsOn = startsOn;
			season.EndsOn = endsOn;
			Assert.IsNotNull(season.Location);
			Assert.IsNull(document.DocumentElement["Address", "Common"]);
			season.Location.Line1 = line1;
			season.Location.City = city;
			season.Location.State = state;
			season.Location.ZipCode = zipCode;
			season.Teams[0].Name = team1Name;
			season.Teams[0].Balance = team1Balance;
			var team3 = season.Teams.AddNew();
			team3.Name = team3Name;
			team3.Balance = team3Balance;
			season.Notes = notes;

			Assert.AreEqual(name, season.Name);
			Assert.AreEqual(minAge, season.MinimumAge);
			Assert.AreEqual(division, season.Division);
			Assert.AreEqual(startsOn.Date, season.StartsOn.Date);
			Assert.AreEqual(endsOn.Date, season.EndsOn.Date);
			Assert.AreEqual(line1, season.Location.Line1);
			Assert.AreEqual(city, season.Location.City);
			Assert.AreEqual(state, season.Location.State);
			Assert.AreEqual(zipCode, season.Location.ZipCode);
			Assert.AreEqual(3, season.Teams.Count);
			Assert.AreEqual(team1Name, season.Teams[0].Name);
			Assert.AreEqual(team1Balance, season.Teams[0].Balance);
			Assert.AreEqual(team3Name, season.Teams[2].Name);
			Assert.AreEqual(team3Balance, season.Teams[2].Balance);
			Assert.AreEqual(notes, season.Notes);

			season.Teams.RemoveAt(1);
			Assert.AreEqual(2, season.Teams.Count);
			Assert.AreEqual(team3Name, season.Teams[1].Name);
			Assert.AreEqual(team3Balance, season.Teams[1].Balance);
		}

		[Test]
		public void Adapter_OnXml_CanCopySubTree()
		{
			var xml = string.Format(
				@"<Season xmlns='RISE' xmlns:rise='RISE'>
					 <Name>Soccer Adult Spring II 2010</Name>
					 <MinimumAge>16</MinimumAge>
					 <Division>Male</Division>
					 <StartsOn>{0}</StartsOn>
					 <EndsOn>{1}</EndsOn>
					 <Address xmlns='Common'>
						<Line1>2922 South Highway 205</Line1>
						<City>Rockwall</City>
						<State>TX</State>
						<ZipCode>75032</ZipCode>
					 </Address>
					 <League>
						<Team name='Hit And Run'>
						   <AmountDue>100.50</AmountDue>
						   <Roster>
							  <Participant FirstName='Mickey' lastName='Mouse'>
							  </Participant>
							  <Participant FirstName='Donald' lastName='Ducks'>
							  </Participant>
						   </Roster>
						</Team>
						<Team name='Nemisis'>
						   <AmountDue>250.00</AmountDue>
						</Team>
					 </League>
					 <Tag>Soccer</Tag>
					 <Tag>Cheetahs</Tag>
					 <Tag>Hot Shots</Tag>
					 <ExtraStuff>
						<LicenseNo>9999</LicenseNo>
					 </ExtraStuff>
				  </Season>",
				XmlConvert.ToString(new DateTime(2010, 7, 19), XmlDateTimeSerializationMode.Local),
				XmlConvert.ToString(new DateTime(2010, 9, 20), XmlDateTimeSerializationMode.Local));

			XmlDocument document1 = null;
			XmlDocument document2 = null;
			var season1 = CreateXmlAdapter<ISeason>(xml, ref document1);
			var season2 = CreateXmlAdapter<ISeason>(null, ref document2);
			season2.Location = season1.Location;
			season2.Tags = season1.Tags;
			season2.Teams = season1.Teams;
				var player = season2.Teams[1].Players.AddNew();
			player.FirstName = "Dave";
			player.LastName = "O'Hara";
			season1.Teams[0].Players[1] = player;

			Assert.AreNotSame(season1.Location, season2.Location);
			Assert.AreEqual(season1.Location.Line1, season2.Location.Line1);
			Assert.AreEqual(season1.Location.City, season2.Location.City);
			Assert.AreEqual(season1.Location.State, season2.Location.State);
			Assert.AreEqual(season1.Location.ZipCode, season2.Location.ZipCode);
			Assert.AreEqual(season1.Tags.Length, season2.Tags.Length);
			Assert.AreEqual(season2.Tags[0], season1.Tags[0]);
			Assert.AreEqual(season2.Tags[1], season1.Tags[1]);
			Assert.AreEqual(season2.Tags[2], season1.Tags[2]);
			Assert.AreEqual(season2.Teams.Count, season1.Teams.Count);
			Assert.AreEqual(season2.Teams[0].Name, season1.Teams[0].Name);
			Assert.AreEqual(season2.Teams[0].Balance, season1.Teams[0].Balance);
			Assert.AreEqual(season2.Teams[0].Players.Count, season2.Teams[0].Players.Count);
			Assert.AreEqual(season2.Teams[0].Players[0].FirstName, season1.Teams[0].Players[0].FirstName);
			Assert.AreEqual(season2.Teams[0].Players[0].LastName, season1.Teams[0].Players[0].LastName);
			Assert.AreEqual(season2.Teams[1].Name, season1.Teams[1].Name);
			Assert.AreEqual(season2.Teams[1].Balance, season1.Teams[1].Balance);
			Assert.AreEqual(player.FirstName, season1.Teams[0].Players[1].FirstName);
			Assert.AreEqual(player.LastName, season1.Teams[0].Players[1].LastName);

			season2.Location = null;
			season2.Tags = null;
			Assert.AreEqual(0, season2.Tags.Length);
		}

		[Test]
		public void Will_Ignore_If_Reassigned_Same_Component()
		{
			var xml = @"<Season xmlns='RISE' xmlns:rise='RISE'>
						 <Address xmlns='Common'>
							<Line1>2922 South Highway 205</Line1>
							<City>Rockwall</City>
							<State>TX</State>
							<ZipCode>75032</ZipCode>
						 </Address>
					 </Season>";

			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(xml, ref document);
			season.Location = season.Location;
			Assert.AreEqual("Rockwall", season.Location.City);
		}

		[Test]
		public void Adapter_OnXml_CanCreate_Other_Adapter()
		{
			var xml = @"<Season xmlns='RISE' xmlns:rise='RISE'>
					 <Name>Soccer Adult Spring II 2010</Name>
					 <MinimumAge>16</MinimumAge>
					 <Division>Male</Division>
					 <League>
						<Team name='Hit And Run'>
						   <AmountDue>100.50</AmountDue>
						</Team>
						<Team name='Nemisis'>
						   <AmountDue>250.00</AmountDue>
						</Team>
					 </League>
				  </Season>";

			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(xml, ref document);
			var address = season.Create<IAddress>();
			Assert.IsNull(address.Line1);
		}

		[Test]
		public void Will_Use_Interface_Name_Without_I_When_Writing()
		{
			XmlDocument document = null;
			var team = CreateXmlAdapter<ITeam>(null, ref document);
			team.Name = "Turfmonsters";
			Assert.AreEqual("Team", document.DocumentElement.LocalName);
			Assert.AreEqual("", document.DocumentElement.NamespaceURI);
		}

		[Test]
		public void Will_Use_TypeName_From_XmlTypeAttribute_When_Writing()
		{
			XmlDocument document = null;
			var player = CreateXmlAdapter<IPlayer>(null, ref document);
			player.FirstName = "Joe";
			Assert.AreEqual("Player", document.DocumentElement.LocalName);
			Assert.AreEqual("", document.DocumentElement.NamespaceURI);
		}

		[Test]
		public void Will_Use_ElementName_And_Namesace_From_XmlRootAttribute_When_Writing()
		{
			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(null, ref document);
			season.Name = "Coed Summer";
			Assert.AreEqual("Season", document.DocumentElement.LocalName);
			Assert.AreEqual("RISE", document.DocumentElement.NamespaceURI);
		}

		[Test, Ignore("Not done")]
		public void Can_Use_XPath_To_Translate_Properties()
		{
			XmlDocument document = null;
			var translator = CreateXmlAdapter<ICanDoTranslations>(null, ref document);
			translator.FirstName = "Billy";
			translator.LastName = "Bob";
			var names = document.SelectNodes("Name/Names");
			Assert.AreEqual(1, names.Count);
			var surnames = document.SelectNodes("Name/Surnames");
			Assert.AreEqual(1, surnames.Count);
		}

		[Test]
		public void Can_Remove_From_Collections()
		{
			var xml = @"<Season xmlns='RISE' xmlns:rise='RISE'>
					 <Name>Soccer Adult Spring II 2010</Name>
					 <MinimumAge>16</MinimumAge>
					 <Division>Male</Division>
					 <League>
						<Team name='Hit And Run' GamesPlayed='2'>
						   <AmountDue>100.50</AmountDue>
						</Team>
						<Team name='Nemisis'>
						   <AmountDue>250.00</AmountDue>
						</Team>
					 </League>
				  </Season>";

			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(xml, ref document);
			foreach (var team in season.Teams.ToArray())
			{
				season.Teams.Remove(team);								
			}
			Assert.AreEqual(0, season.Teams.Count);
			var teams = document.GetElementsByTagName("Team", "RISE");
			Assert.AreEqual(0, teams.Count);
			var league = document.GetElementsByTagName("League", "RISE");
			Assert.AreEqual(1, league.Count);
			Assert.AreEqual(0, league[0].ChildNodes.Count);
		}

		[Test]
		public void Can_Remove_Collections_With_Nil()
		{
			var xml = @"<Season xmlns='RISE' xmlns:rise='RISE'>
						<League>
							<Team name='Hit And Run'>
								<Roster>
									<Participant FirstName='Mickey' lastName='Mouse'>
									</Participant>
									<Participant FirstName='Donald' lastName='Ducks'>
									</Participant>
								</Roster>
								<AmountDue>100.50</AmountDue>
							</Team>
						</League>
					</Season>";

			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(xml, ref document);
			foreach (var player in season.Teams[0].Players.ToArray())
			{
				season.Teams[0].Players.Remove(player);
			}
			Assert.IsNull(season.Teams[0].Players);
			var roster = document.GetElementsByTagName("Roster", "RISE");
			Assert.AreEqual(1, roster.Count);
			Assert.AreEqual(0, roster[0].ChildNodes.Count);
			var nil = roster[0].Attributes["nil", "http://www.w3.org/2001/XMLSchema-instance"];
			Assert.IsNotNull(nil);
			Assert.AreEqual("true", nil.Value);
		}

		[Test]
		public void Can_Clear_Collections()
		{
			var xml = @"<Season xmlns='RISE' xmlns:rise='RISE'>
					 <Name>Soccer Adult Spring II 2010</Name>
					 <MinimumAge>16</MinimumAge>
					 <Division>Male</Division>
					 <League>
						<Team name='Hit And Run' GamesPlayed='2'>
						   <AmountDue>100.50</AmountDue>
						</Team>
						<Team name='Nemisis'>
						   <AmountDue>250.00</AmountDue>
						</Team>
					 </League>
				  </Season>";

			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(xml, ref document);
			season.Teams.Clear();
			Assert.AreEqual(0, season.Teams.Count);
			var teams = document.GetElementsByTagName("Team", "RISE");
			Assert.AreEqual(0, teams.Count);
		}

		[Test]
		public void Can_Clear_Empty_Collections()
		{
			var xml = @"<Season xmlns='RISE' xmlns:rise='RISE'>
					 <Name>Soccer Adult Spring II 2010</Name>
					 <MinimumAge>16</MinimumAge>
					 <Division>Male</Division>
				  </Season>";

			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(xml, ref document);
			season.Teams.Clear();
			var teams = document.GetElementsByTagName("Team", "RISE");
			Assert.AreEqual(0, teams.Count);
		}

		[Test]
		public void Can_Clear_Collections_With_Nil()
		{
				var xml = @"<Season xmlns='RISE' xmlns:rise='RISE'>
						<League>
							<Team name='Hit And Run'>
								<Roster>
									<Participant FirstName='Mickey' lastName='Mouse'>
									</Participant>
									<Participant FirstName='Donald' lastName='Ducks'>
									</Participant>
								</Roster>
								<AmountDue>100.50</AmountDue>
							</Team>
						</League>
					</Season>";

			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(xml, ref document);
			season.Teams[0].Players.Clear();
			Assert.IsNull(season.Teams[0].Players);
			var roster = document.GetElementsByTagName("Roster", "RISE");
			Assert.AreEqual(1, roster.Count);
			Assert.AreEqual(0, roster[0].ChildNodes.Count);
			var nil = roster[0].Attributes["nil", "http://www.w3.org/2001/XMLSchema-instance"];
			Assert.IsNotNull(nil);
			Assert.AreEqual("true", nil.Value);
		}

		[Test]
		public void Can_Read_Nullable_Attribute()
		{
			var xml = @"<Season xmlns='RISE' xmlns:rise='RISE'>
					 <Name>Soccer Adult Spring II 2010</Name>
					 <MinimumAge>16</MinimumAge>
					 <Division>Male</Division>
					 <League>
						<Team name='Hit And Run' GamesPlayed='2'>
						   <AmountDue>100.50</AmountDue>
						</Team>
						<Team name='Nemisis'>
						   <AmountDue>250.00</AmountDue>
						</Team>
					 </League>
				  </Season>";

			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(xml, ref document);
			var team = season.Teams[0];
			Assert.AreEqual(2, team.GamesPlayed);
		}

		[Test]
		public void Can_Read_Nullable_Element()
		{
			var xml = @"<Season xmlns='RISE' xmlns:rise='RISE'>
					 <Name>Soccer Adult Spring II 2010</Name>
					 <MinimumAge>16</MinimumAge>
					 <Division>Male</Division>
					 <League>
						<Team name='Hit And Run'>
						   <MaxPlayers>15</MaxPlayers>
						   <AmountDue>100.50</AmountDue>
						</Team>
						<Team name='Nemisis'>
						   <AmountDue>250.00</AmountDue>
						</Team>
					 </League>
				  </Season>";

			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(xml, ref document);
			var team = season.Teams[0];
			Assert.AreEqual(15, team.MaxPlayers);
		}

		[Test]
		public void Can_Write_Nullable_Attribute()
		{
			var xml = @"<Season xmlns='RISE' xmlns:rise='RISE'>
					 <Name>Soccer Adult Spring II 2010</Name>
					 <MinimumAge>16</MinimumAge>
					 <Division>Male</Division>
					 <League>
						<Team name='Hit And Run'>
						   <AmountDue>100.50</AmountDue>
						</Team>
						<Team name='Nemisis'>
						   <AmountDue>250.00</AmountDue>
						</Team>
					 </League>
				  </Season>";

			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(xml, ref document);
			var team = season.Teams[0];
			team.GamesPlayed = 2;
			Assert.AreEqual(2, team.GamesPlayed);
		}

		[Test]
		public void Can_Write_Nullable_Element()
		{
			var xml = @"<Season xmlns='RISE' xmlns:rise='RISE'>
					 <Name>Soccer Adult Spring II 2010</Name>
					 <MinimumAge>16</MinimumAge>
					 <Division>Male</Division>
					 <League>
						<Team name='Hit And Run'>
						   <AmountDue>100.50</AmountDue>
						</Team>
						<Team name='Nemisis'>
						   <AmountDue>250.00</AmountDue>
						</Team>
					 </League>
				  </Season>";

			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(xml, ref document);
			var team = season.Teams[0];
			team.MaxPlayers = 15;
			Assert.AreEqual(15, team.MaxPlayers);
		}

		[Test]
		public void Can_Force_Namespace_Qualification()
		{
			XmlDocument document = null;
			var tags = CreateXmlAdapter<ITagged>(null, ref document);
			tags.Tags = new[] { "Quick", "Smart" };
			var taggedNode = document["Tagged", ""];
			var tagsNode = taggedNode.GetElementsByTagName("string", "urn:www.castle.org:tags");
			var t = tagsNode.Cast<XmlNode>().Select(node => node.InnerText).ToArray();
			CollectionAssert.AreEqual(tags.Tags, t);
		}

		[Test]
		public void Can_Promote_Namespace_To_Document()
		{
			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(null, ref document);
			season.Location.Line1 = "100 Hershey Park";
			season.Location.City = "Hershey";
			season.Location.State = "PA";
			Assert.AreEqual("c", document.DocumentElement.GetPrefixOfNamespace("Common"));
		}

		[Test]
		public void Can_Coerce_Xml_With_Namespace()
		{
			var xml = @"<Season xmlns='RISE'>
					 <Address xmlns='Common'>
						<Line1>2922 South Highway 205</Line1>
					 </Address>
				  </Season>";

			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(xml, ref document);
			var tagged = season.Coerce<ITagged>();
			tagged.Tags = new[] { "Primary", "Soccer" };

			var seasonNode = document["Season", "RISE"];
			var tagsNode = seasonNode["Tags", "urn:www.castle.org:tags"];
			Assert.IsNotNull(tagsNode);
		}

		[Test]
		public void Can_Coerce_Xml_Without_Namespace()
		{
			XmlDocument document = null;
			var team = CreateXmlAdapter<ITeam>(null, ref document);
			team.Name = "Fairly OddParents";
			var tagged = ((IDictionaryAdapter)team).Coerce<ITagged>();
			tagged.Tags = new[] { "Primary", "Soccer" };
			team.Balance = 200.00M;

			var teamNode = document["Team", ""];
			Assert.AreEqual("200.00", teamNode["AmountDue", ""].InnerText);
			var tagsNode = teamNode["Tags", "urn:www.castle.org:tags"];
			Assert.IsNotNull(tagsNode);
		}

		[Test]
		public void Can_Determine_If_Property_Defined_In_Xml()
		{
			var xml = @"<Season xmlns='RISE'>
					 <Address xmlns='Common'>
						<Line1>2922 South Highway 205</Line1>
					 </Address>
				  </Season>";

			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(xml, ref document);
			Assert.IsTrue(XmlAdapter.IsPropertyDefined("Location", season));
			Assert.IsFalse(XmlAdapter.IsPropertyDefined("Name", season));
		}

		[Test]
		public void Can_Determine_If_Collection_Defined_In_Xml()
		{
			var xml = @"<Season xmlns='RISE' xmlns:rise='RISE'>
					 <League>
						<Team name='Hit And Run'>
						   <AmountDue>100.50</AmountDue>
						</Team>
						<Team name='Nemisis'>
						   <AmountDue>250.00</AmountDue>
						</Team>
					 </League>
				  </Season>";

			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(xml, ref document);
			Assert.IsTrue(XmlAdapter.IsPropertyDefined("Teams", season));

			// TODO: Ask Craig what he wants to do about this
			// In my POV, XmlElementBehavior means that collections are ALWAYS defined
			//   since there is not an element representing the collection itself.
//			Assert.IsFalse(XmlAdapter.IsPropertyDefined("Tags", season));
			Assert.IsTrue(XmlAdapter.IsPropertyDefined("Tags", season));
		}

		[Test]
		public void Can_Reassign_Arrays()
		{
			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(null, ref document);
			season.Tags = new[] { "Hello", "Goodbye" };
			season.Tags = new[] { "Alpha", "Beta" };
			CollectionAssert.AreEqual(new[] { "Alpha", "Beta" }, season.Tags);
		}

		[Test]
		public void Can_Reassign_Lists()
		{
			var xml = @"<Season xmlns='RISE' xmlns:rise='RISE'>
					 <League>
						<Team name='Hit And Run'>
						   <AmountDue>100.50</AmountDue>
						</Team>
						<Team name='Nemisis'>
						   <AmountDue>250.00</AmountDue>
						</Team>
					 </League>
				  </Season>";

			XmlDocument document = null, teamDoc = null;
			var season = CreateXmlAdapter<ISeason>(xml, ref document);
			var team = CreateXmlAdapter<ITeam>(null, ref teamDoc);
			team.Name = "Turfmonsters";
			var newTeams = new BindingList<ITeam>();
			newTeams.Add(team);
			season.Teams = newTeams;
			Assert.AreEqual(1, season.Teams.Count);
		}

		[Test]
		public void Can_Remove_Properties()
		{
			var xml = @"<Season xmlns='RISE' xmlns:rise='RISE'>
					      <Name>Soccer Adult Spring II 2010</Name>
				        </Season>";

			XmlDocument document = null;
			var season = CreateXmlAdapter<ISeason>(xml, ref document);
			Assert.IsTrue(XmlAdapter.IsPropertyDefined("Name", season));
			season.Name = "";
			Assert.IsFalse(XmlAdapter.IsPropertyDefined("Name", season));
		}

#if !DOTNET35
		[Test]
		public void Can_Detect_Circularities()
		{
			var starWars = new[] 
			{ 
				"Star Wars Episode IV: A New Hope",
				"Star Wars Episode V: The Empire Strikes Back",
				"Star Wars Episode VI: Return of the Jedi",
				"Star Wars Episode I: The Phantom Menace",
				"Star Wars Episode II: Attack of the Clones",
				"Star Wars Episode III: Revenge of the Sith",
				"Star Wars: The Clone Wars"
			};

			var books = starWars.Select((title, i) =>
			{
				XmlDocument bookDoc = null;
				var book = CreateXmlAdapter<IBook>(null, ref bookDoc);
				book.Title = title;
				book.DDC.Category = 8;
				book.DDC.SubCategory = 1;
				book.DDC.SubDivision = i;
				book.Printed = i % 2 == 0;
				return book;
			}).ToList();

			foreach (var book in books)
			{
				var relatedBooks = new HashSet<IBook>(books.Except(Enumerable.Repeat(book, 1)));
				book.RelatedBooks = relatedBooks;
			}
		}
#endif

		private T CreateXmlAdapter<T>(string xml, ref XmlDocument document)
		{
			document = document ?? new XmlDocument();
			if (xml != null)
			{
				document.LoadXml(xml);
			}
			return (T)factory.GetAdapter(typeof(T), document);
		}

		public enum Division
		{
			Male,
			Female,
			Coed
		}

		[XmlNamespace("Common", "c", Root = true),
		 XmlNamespace("RISE", "r"),
		 XPath("r:Season/c:Address")]
		public interface IAddress
		{
			[Volatile]
			string Line1 { get; set; }
			string City { get; set; }
			string State { get; set; }
			string ZipCode { get; set; }
		}

		[XmlType("Player", Namespace = "People")]
		public interface IPlayer
		{
			string FirstName { get; set; }
			string LastName { get; set; }
		}

		[XmlType("Goalie", Namespace = "People")]
		public interface IGoalie : IPlayer
		{
			int GoalAllowed { get; set; }
		}

		public interface ITeam
		{
			[XmlAttribute]
			string Name { get; set; }
			[XmlAttribute]
			int? GamesPlayed { get; set; }
			int? MaxPlayers { get; set; }
			[XmlElement("AmountDue")]
			decimal Balance { get; set; }
			[XmlArray("Roster", IsNullable = true), XmlArrayItem("Participant"), RemoveIfEmpty]
			IBindingList<IPlayer> Players { get; }
		}

		[XmlRoot("Season", Namespace = "RISE"),
		 XmlNamespace("RISE", "rise", Default = true),
		 XmlNamespace("Common", "c", Root = true)]
		public interface ISeason : IDictionaryAdapter
		{
			[RemoveIf("")]
			string Name { get; set; }
			int MinimumAge { get; set; }
			Division Division { get; set; }
			DateTime StartsOn { get; set; }
			DateTime EndsOn { get; set; }
			[XPath("sum(rise:League/rise:Team/rise:AmountDue)")]
			decimal Balance { get; }
			[XmlElement("Address", Namespace = "Common")]
			IAddress Location { get; set; }
			[Key("League"), XmlArrayItem("Team")]
			IBindingList<ITeam> Teams { get; set; }
			[XPath("rise:League/rise:Team")]
			ITeam[] TeamsArray { get; }
			[XPath("rise:League/rise:Team[position()=1]/@Name")]
			string FirstTeamName { get; }
			[XPath("Documentation/Notes")]
			string Notes { get; set; }
			[XmlElement("Tag")]
			string[] Tags { get; set; }
			XmlElement ExtraStuff { get; set; }
		}

		public interface IName
		{
			string[] Names { get; set; }
			string[] Surnames { get; set; }
		}

		public interface ICanDoTranslations
		{
			[XPath("Name/Names/*")]
			string FirstName { get; set; }

			[XPath("Name/Surnames/*")]
			string LastName { get; set; }
		}

		[XmlDefaults(Qualified = true),
		 XmlNamespace("urn:www.castle.org:tags", "tags"),
		 XmlType(Namespace = "urn:www.castle.org:tags")]
		public interface ITagged
		{
			string[] Tags { get; set; }
		}

		[Test]
		public void Can_Read_From_Standard_Xml_Serialization()
		{
			var manager = new Manager { Name = "Craig", Level = 1 };
			var employee = new Employee
			{
				Name = "Dave",
				Supervisor = manager,
				Job = new Employment { Title = "Consultant", Salary = 100000M },
				Metadata = new Metadata { Tag = "Cool!" },
				Key = new byte[] { 0x01, 0x02, 0x03 }
			};
			var group = new Group
			{
				Id = 2,
				Owner = manager,
				Employees = new Employee[] { employee, manager },
				Tags = new[] { "Primary", "Local" },
				Codes = Enumerable.Range(1, 5).ToList(),
				Comment = "Nothing important",
				ExtraInfo = new object[] { 43, "Extra", manager }
			};

			using (var stream = new FileStream("out.xml", FileMode.Create))
			{
				var serializer = new XmlSerializer(typeof(Group));
				serializer.Serialize(stream, group);
				stream.Flush();
			}

			var document = new XmlDocument();
			document.Load("out.xml");

			var groupRead = CreateXmlAdapter<IGroup>(null, ref document);
			Assert.AreEqual(2, groupRead.Id);
			Assert.IsInstanceOf<IManager>(groupRead.Owner);
			var managerRead = (IManager)groupRead.Owner;
			Assert.AreEqual(manager.Name, managerRead.Name);
			Assert.AreEqual(manager.Level, managerRead.Level);
			var employeesRead = groupRead.Employees;
			Assert.AreEqual(2, employeesRead.Length);
			Assert.AreEqual(employee.Name, employeesRead[0].Name);
			Assert.AreEqual(employee.Job.Title, employeesRead[0].Job.Title);
			Assert.AreEqual(employee.Job.Salary, employeesRead[0].Job.Salary);
			Assert.AreEqual(employee.Metadata.Tag, employeesRead[0].Metadata.Tag);
			CollectionAssert.AreEqual(employee.Key, employeesRead[0].Key);
			Assert.AreEqual(manager.Name, employeesRead[1].Name);
			Assert.IsInstanceOf<IManager>(employeesRead[1]);
			var managerEmplRead = (IManager)employeesRead[1];
			Assert.AreEqual(manager.Level, managerEmplRead.Level);
			CollectionAssert.AreEqual(group.Tags, groupRead.Tags);
			var extraInfoRead = groupRead.ExtraInfo;
			Assert.AreEqual(3, extraInfoRead.Length);
			Assert.AreEqual(group.ExtraInfo[0], extraInfoRead[0]);
			Assert.AreEqual(group.ExtraInfo[1], extraInfoRead[1]);
			Assert.IsInstanceOf<IManager>(extraInfoRead[2]);
			var managerExtra = (IManager)extraInfoRead[2];
			Assert.AreEqual(manager.Name, managerExtra.Name);
			Assert.AreEqual(manager.Level, managerExtra.Level);

			groupRead.Comment = "Hello World";
			Assert.AreEqual("Hello World", groupRead.Comment);
			var commentRead = document["Comment", "Yum"];
			Assert.IsNull(commentRead);
		}

		[Test]
		public void Can_Read_Nillables_From_Standard_Xml_Serialization()
		{
			var manager = new Manager { Name = "Craig", Level = 1 };
			var group = new Group
			{
				Id = 2,
				Employees = new Employee[] { null, manager }
			};

			using (var stream = new FileStream("out.xml", FileMode.Create))
			{
				var serializer = new XmlSerializer(typeof(Group));
				serializer.Serialize(stream, group);
				stream.Flush();
			}

			var document = new XmlDocument();
			document.Load("out.xml");

			var groupRead = CreateXmlAdapter<IGroup>(null, ref document);
			Assert.IsNull(groupRead.Owner);
			Assert.IsNull(groupRead.ExtraInfo);
			Assert.AreEqual(1, groupRead.Employees.Length);
		}

		[Test, Ignore("Need to opt in or out of reference tracking for this to work.")]
		public void Can_Write_To_Standard_Xml_Serialization()
		{
			XmlDocument document = null, mgr = null, emp = null;
			var manager = CreateXmlAdapter<IManager>(null, ref mgr);
			manager.Name = "Craig";
			manager.Level = 1;

			var employee = CreateXmlAdapter<IEmployee>(null, ref emp);
			employee.Name = "Dave";
			employee.Supervisor = manager;
			employee.Job = new Employment
			{
				Title = "Consultant",
				Salary = 100000M
			};
			employee.Metadata = new Metadata { Tag = "Cool!" };
			employee.Key = new byte[] { 0x01, 0x02, 0x03 };

			var group = CreateXmlAdapter<IGroup>(null, ref document);
			group.Id = 2;
			group.Owner = manager;
			group.Employees = new IEmployee[] { employee, manager };
			group.Tags = new[] { "Primary", "Local" };
			group.Codes = Enumerable.Range(1, 5).ToList();
			group.Comment = "Nothing important";
			group.ExtraInfo = new object[] { 43, "Extra", manager };

			document.Save("out.xml");

			using (var stream = new FileStream("out.xml", FileMode.Open))
			{
				var serializer = new XmlSerializer(typeof(Group));
				var groupRead = (Group)serializer.Deserialize(stream);

				Assert.AreEqual(2, groupRead.Id);
				Assert.IsInstanceOf<Manager>(groupRead.Owner);
				var managerRead = (Manager)groupRead.Owner;
				Assert.AreEqual(manager.Name, managerRead.Name);
				Assert.AreEqual(manager.Level, managerRead.Level);
				var employeesRead = groupRead.Employees;
				Assert.AreEqual(2, employeesRead.Length);
				Assert.AreEqual(employee.Name, employeesRead[0].Name);
				Assert.AreEqual(employee.Job.Title, employeesRead[0].Job.Title);
				Assert.AreEqual(employee.Job.Salary, employeesRead[0].Job.Salary);
				Assert.AreEqual(employee.Metadata.Tag, employeesRead[0].Metadata.Tag);
				CollectionAssert.AreEqual(employee.Key, employeesRead[0].Key);
				Assert.AreEqual(manager.Name, employeesRead[1].Name);
				Assert.IsInstanceOf<Manager>(employeesRead[1]);
				var managerEmplRead = (Manager)employeesRead[1];
				Assert.AreEqual(manager.Level, managerEmplRead.Level);
				CollectionAssert.AreEqual(group.Tags, groupRead.Tags);
				Assert.IsNull(groupRead.Comment);
				Assert.AreEqual(3, groupRead.ExtraInfo.Length);
				Assert.AreEqual(group.ExtraInfo[0], groupRead.ExtraInfo[0]);
				Assert.AreEqual(group.ExtraInfo[1], groupRead.ExtraInfo[1]);
				Assert.IsInstanceOf<Manager>(groupRead.ExtraInfo[2]);
				var managerExtra = (Manager)groupRead.ExtraInfo[2];
				Assert.AreEqual(manager.Name, managerExtra.Name);
				Assert.AreEqual(manager.Level, managerExtra.Level);
			}
		}

		[Test]
		public void Can_Write_Nillables_To_Standard_Xml_Serialization()
		{
			XmlDocument document = null, mgr = null;
			var manager = CreateXmlAdapter<IManager>(null, ref mgr);
			manager.Name = "Craig";
			manager.Level = 1;
			var group = CreateXmlAdapter<IGroup>(null, ref document);
			group.Id = 2;
			group.Owner = null;
			group.Employees = new IEmployee[] { null, manager };

			document.Save("out.xml");

			using (var stream = new FileStream("out.xml", FileMode.Open))
			{
				var serializer = new XmlSerializer(typeof(Group));
				var groupRead = (Group)serializer.Deserialize(stream);
				Assert.IsNull(groupRead.Owner);
			}
		}

		[Test]
		public void Will_Return_Null_if_Must_Exist()
		{
			XmlDocument document = null;
			var employee = CreateXmlAdapter<IEmployee>(null, ref document);
			Assert.IsNull(employee.Supervisor);
		}
	}

	#region DA Serialization Model

	[XmlType(TypeName = "Groupy", Namespace = "Yum"),
	 XmlRoot(ElementName = "GroupRoot", Namespace = "Arg")]
	public interface IGroup
	{
		int Id { get; set; }

		[XmlElement(IsNullable = true)]
		IEmployee Owner { get; set; }

		[XmlArrayItem("Dude", Type = typeof(IEmployee), IsNullable = true),
		 XmlArrayItem(Type = typeof(IManager), IsNullable = true)]
		IEmployee[] Employees { get; set; }

		string[] Tags { get; set; }

		IList<int> Codes { get; set; }

		[XmlIgnore]
		string Comment { get; set; }

		[XmlArray(IsNullable = true),
		 XmlArrayItem(typeof(int), ElementName = "MyNumber"),
		 XmlArrayItem(typeof(string), ElementName = "MyString"),
		 XmlArrayItem(typeof(IManager))]
		object[] ExtraInfo { get; set; }
	}

	//[XmlRoot(ElementName = "FooRoot")]
	[XmlType(TypeName = "Foo", Namespace = "Something"),
	 XmlInclude(typeof(IManager))]
	public interface IEmployee
	{
		string Name { get; set; }
		[IfExists]
		IEmployee Supervisor { get; set; }
		Employment Job { get; set; }
		Metadata Metadata { get; set; }
		byte[] Key { get; set; }
	}

	//[XmlRoot(ElementName = "BarRoot")]
	[XmlType(TypeName = "Bar", Namespace = "Nothing")]
	public interface IManager : IEmployee
	{
		int Level { get; set; }
	}
	
	#endregion

	#region Xml Serialization Model

	[XmlType(TypeName = "Groupy", Namespace = "Yum"),
	 XmlRoot(ElementName = "GroupRoot", Namespace = "Arg")]
	public class Group
	{
		public int Id;

		[XmlElement(IsNullable = true)]
		public Employee Owner;

		[XmlArrayItem("Dude", Type = typeof(Employee), IsNullable = true),
		 XmlArrayItem(Type = typeof(Manager), IsNullable = true)]
		public Employee[] Employees;

		public string[] Tags;

		public List<int> Codes;

		[XmlIgnore]
		public string Comment;

		[XmlArray(IsNullable = true),
		 XmlArrayItem(typeof(int), ElementName = "MyNumber"),
		 XmlArrayItem(typeof(string), ElementName = "MyString"),
		 XmlArrayItem(typeof(Manager))]
		public object[] ExtraInfo;
	}

	//[XmlRoot(ElementName = "FooRoot")]
	[XmlType(TypeName="Foo", Namespace = "Something"),
	 XmlInclude(typeof(Manager))]
	public class Employee
	{
		public string Name;
		public Employee Supervisor;
		public Employment Job;
		public Metadata Metadata;
		public byte[] Key;
	}

	//[XmlRoot(ElementName = "BarRoot")]
	[XmlType(TypeName = "Bar", Namespace = "Nothing")]
	public class Manager : Employee
	{
		public int Level;
	}

	[XmlType(Namespace = "Potato"),
	 XmlRoot(Namespace = "Pickle")]
	public class Employment
	{
		public string Title { get; set; }

		public decimal Salary { get; set; }
	}

	public class Metadata : IXmlSerializable
	{
		public string Tag { get; set; }

		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			var isEmptyElement = reader.IsEmptyElement;
			reader.ReadStartElement();
			if (isEmptyElement == false) 
			{
				Tag = reader.ReadElementString("Tag");
				reader.ReadEndElement();
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			if (string.IsNullOrEmpty(Tag) == false)
			{
				writer.WriteElementString("Tag", Tag);
			}
		}

		public XmlSchema GetSchema()
		{
			return null;
		}
	}

	#endregion
}
