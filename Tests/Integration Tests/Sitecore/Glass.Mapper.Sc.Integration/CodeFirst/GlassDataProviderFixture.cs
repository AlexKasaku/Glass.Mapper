﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Glass.Mapper.Sc.CastleWindsor;
using Glass.Mapper.Sc.CodeFirst;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Configuration.Fluent;
using Glass.Mapper.Sc.Integration.CodeFirst.Templates.GlassDataProvider;
using NUnit.Framework;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.DataProviders;
using Sitecore.Data.Items;

namespace Glass.Mapper.Sc.Integration.CodeFirst
{
    [TestFixture]
    public class GlassDataProviderFixture
    {
        [SetUp]
        public void Setup()
        {
            //remove provider from database
            var db = Sitecore.Configuration.Factory.GetDatabase("master");

            var providers = GetProviders(db);
            var toRemove = providers.Where(x => x is GlassDataProvider).ToList();
            toRemove.ForEach(x => providers.Remove(x));
            
            var path = "/sitecore/templates/glasstemplates";
            var rootFolder = db.GetItem(path);
            if (rootFolder != null)
                rootFolder.Delete();



        }


        [Test]
        public void GlassDataProvider_ReturnsGlassTemplateFolder()
        {
            //Assign
            var db = Sitecore.Configuration.Factory.GetDatabase("master");

            var dataProvider = new GlassDataProvider("master", Context.DefaultContextName);

            dataProvider.Initialise(db);

            InjectionDataProvider(db, dataProvider);

            var context = Context.Create(Utilities.CreateStandardResolver());

            var path = "/sitecore/templates/glasstemplates";

            db.Caches.DataCache.Clear();
            db.Caches.ItemCache.Clear();
            db.Caches.ItemPathsCache.Clear();
            db.Caches.StandardValuesCache.Clear();
            db.Caches.PathCache.Clear();
            //Act
            var folder = db.GetItem(path);

            var tempFolder = db.GetItem("/sitecore/templates");

            foreach (Item item in tempFolder.Children)
            {
                Console.WriteLine(item.Name);
            }

            //Assert
            Assert.AreEqual(folder.Name, "GlassTemplates");

        }

        [Test]
        public void GlassDataProvider_ReturnsTemplate()
        {
            //Assign

            var db = Sitecore.Configuration.Factory.GetDatabase("master");
            var context = Context.Create(Utilities.CreateStandardResolver());

            var loader = new SitecoreFluentConfigurationLoader();

            loader.Add<CodeFirstClass1>()
                  .TemplateId("D3595652-AC29-4BD4-A8D7-DD2120EE6460")
                  .TemplateName("CodeFirstClass1")
                  .CodeFirst();

            context.Load(loader);

            var path = "/sitecore/templates/glasstemplates/CodeFirstClass1";

            var dataProvider = new GlassDataProvider("master", Context.DefaultContextName);

            dataProvider.Initialise(db);

            var master = Sitecore.Configuration.Factory.GetDatabase("master");

            InjectionDataProvider(master, dataProvider);


            //Act
            var folder = db.GetItem(path);


            string xml = Sitecore.Configuration.Factory.GetConfiguration().OuterXml;
            //Assert
            Assert.AreEqual(folder.Name, "CodeFirstClass1");

        }

        [Test]
        public void GlassDataProvider_TemplateInNamespace_ReturnsTemplate()
        {
            //Assign

            var db = Sitecore.Configuration.Factory.GetDatabase("master");
            var context = Context.Create(Utilities.CreateStandardResolver());

            var loader = new SitecoreFluentConfigurationLoader();

            loader.Add<CodeFirstClass2>()
                  .TemplateId("E33F1C58-FAB2-475A-B2FE-C26F5D7565A2")
                  .TemplateName("CodeFirstClass2")
                  .CodeFirst();

            context.Load(loader);

            var path = "/sitecore/templates/glasstemplates/GlassDataProvider/CodeFirstClass2";

            var dataProvider = new GlassDataProvider("master", Context.DefaultContextName);

            dataProvider.Initialise(db);

            var master = Sitecore.Configuration.Factory.GetDatabase("master");

            InjectionDataProvider(master, dataProvider);


            //Act
            var folder = db.GetItem(path);


            string xml = Sitecore.Configuration.Factory.GetConfiguration().OuterXml;
            //Assert
            Assert.AreEqual(folder.Name, "CodeFirstClass2");

        }

        [Test]
        public void GlassDataProvider_ReturnsTemplateWithSectionAndField()
        {
            //Assign

            var db = Sitecore.Configuration.Factory.GetDatabase("master");
            var context = Context.Create(Utilities.CreateStandardResolver());

            var loader = new SitecoreFluentConfigurationLoader();

            loader.Add<CodeFirstClass1>()
                  .TemplateId("D3595652-AC29-4BD4-A8D7-DD2120EE6460")
                  .TemplateName("CodeFirstClass1")
                  .CodeFirst()
                  .Fields(x =>
                          x.Field(y => y.Field1)
                           .IsCodeFirst()
                           .FieldId("32FE1520-EAD4-4CF8-A69F-A4717E2F07F6")
                           .SectionName("TestSection")
                );



            context.Load(loader);

            var path = "/sitecore/templates/glasstemplates/CodeFirstClass1";



            var dataProvider = new GlassDataProvider("master", Context.DefaultContextName);

            dataProvider.Initialise(db);

            var master = Sitecore.Configuration.Factory.GetDatabase("master");

            InjectionDataProvider(master, dataProvider);


            //Act
            var folder = db.GetItem(path);
            
            //Assert
            Assert.AreEqual(folder.Name, "CodeFirstClass1");
            var section = folder.Children.FirstOrDefault(x => x.Name == "TestSection");
            Assert.IsNotNull(section);
            var field = section.Children.FirstOrDefault(x => x.Name == "Field1");
            Assert.IsNotNull(field);
             
        }

        [Test]
        public void GlassDataProvider_ReturnsTemplateWithSectionAndField_AllPropertiesSet()
        {
            //Assign

            var db = Sitecore.Configuration.Factory.GetDatabase("master");
            var context = Context.Create(Utilities.CreateStandardResolver());

            var loader = new SitecoreFluentConfigurationLoader();

            var fieldId = new ID("32FE1520-EAD4-4CF8-A69F-A4717E2F07F6");
            var sectionName = "TestSection";
            var fieldSortOrder = 123;
            var fieldName = "FieldName";
            var fieldTitle = "TestTitle";
            var fieldSource = "/source";
            var fieldType = SitecoreFieldType.Date;
            var sectionSortOrder = 456;
            var validationErrorText = "TextValidation";
            var validationRegEx = "testregex";

            loader.Add<CodeFirstClass1>()
                  .TemplateId("D3595652-AC29-4BD4-A8D7-DD2120EE6460")
                  .TemplateName("CodeFirstClass1")
                  .CodeFirst()
                  .Fields(x =>
                          x.Field(y => y.Field1)
                           .IsCodeFirst()
                           .FieldId(fieldId.ToString())
                           .SectionName(sectionName)
                           .FieldSortOrder(fieldSortOrder)
                           .FieldName(fieldName)
                           .FieldSource(fieldSource)
                           .FieldTitle(fieldTitle)
                           .FieldType(fieldType)
                           .IsRequired()
                           .IsShared()
                           .IsUnversioned()
                           .SectionSortOrder(sectionSortOrder)
                           .ValidationErrorText(validationErrorText)
                           .ValidationRegularExpression(validationRegEx)
                );



            context.Load(loader);

            var path = "/sitecore/templates/glasstemplates/CodeFirstClass1";
            var master = Sitecore.Configuration.Factory.GetDatabase("master");

            var dataProvider = new GlassDataProvider("master", Context.DefaultContextName);

            dataProvider.Initialise(db);

            InjectionDataProvider(master, dataProvider);

            //Act
            var folder = db.GetItem(path);

            //Assert
            Assert.AreEqual(folder.Name, "CodeFirstClass1");
            var section = folder.Children.FirstOrDefault(x => x.Name == sectionName);
            Assert.IsNotNull(section);
            Assert.AreEqual(sectionSortOrder.ToString(), section[FieldIDs.Sortorder]); 

            var field = section.Children.FirstOrDefault(x => x.Name == fieldName);
            Assert.IsNotNull(field);
            Assert.AreEqual(fieldSortOrder.ToString(), field[FieldIDs.Sortorder]); 
            Assert.AreEqual(fieldId, field.ID);
            Assert.AreEqual(fieldSource, field[TemplateFieldIDs.Source]);
            Assert.AreEqual(fieldTitle, field[TemplateFieldIDs.Title]);
            Assert.AreEqual(fieldType.ToString(), field[TemplateFieldIDs.Type]);
            Assert.AreEqual(Global.IDs.TemplateFieldIds.IsRequiredId, field[Global.IDs.TemplateFieldIds.ValidateButtonFieldId]);
            Assert.AreEqual(Global.IDs.TemplateFieldIds.IsRequiredId, field[Global.IDs.TemplateFieldIds.WorkflowFieldId]);
            Assert.AreEqual(Global.IDs.TemplateFieldIds.IsRequiredId, field[Global.IDs.TemplateFieldIds.ValidatorBarFieldId]);
            Assert.AreEqual(Global.IDs.TemplateFieldIds.IsRequiredId, field[Global.IDs.TemplateFieldIds.QuickActionBarFieldId]);
            Assert.AreEqual("1", field[TemplateFieldIDs.Shared]);
            Assert.AreEqual("1", field[TemplateFieldIDs.Unversioned]);
            Assert.AreEqual(validationErrorText, field[TemplateFieldIDs.ValidationText]);
            Assert.AreEqual(validationRegEx, field[TemplateFieldIDs.Validation]);



        }

        public class CodeFirstClass1
        {
            public virtual string Field1 { get; set; }
        }

        private void InjectionDataProvider(Database db, DataProvider provider)
        {
            var providers = GetProviders(db);

            providers.Insert(0,provider);
            //var addMethod = typeof(Database).GetMethod("AddDataProvider", BindingFlags.NonPublic | BindingFlags.Instance);
            //addMethod.Invoke(db, new[] { provider });
        }

        public DataProviderCollection GetProviders(Database db )
        {
            var providersField = typeof(Database).GetField("_dataProviders", BindingFlags.NonPublic | BindingFlags.Instance);
            var providers = providersField.GetValue(db) as DataProviderCollection;
            return providers;
        }
        
    
}

    namespace Templates.GlassDataProvider
    {
        public class CodeFirstClass2{}
    }
}


