﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.ReferenceData.Api.Client;
using FluentAssertions;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.EAS.Infrastructure.Caching;
using SFA.DAS.Common.Domain.Types;
using Organisation = SFA.DAS.ReferenceData.Api.Client.Dto.Organisation;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.ReferenceDataService
{

    public class WhenISearchAndGetCloseMatches
    {
        private Mock<IReferenceDataApiClient> _apiClient;
        private Infrastructure.Services.ReferenceDataService _referenceDataService;
        private Mock<IMapper> _mapper;
        private Mock<ICacheProvider> _cacheProvider;

        [SetUp]
        public void Arrange()
        {
            _apiClient = new Mock<IReferenceDataApiClient>();
            _mapper = new Mock<IMapper>();
            _cacheProvider = new Mock<ICacheProvider>();

            _referenceDataService = new Infrastructure.Services.ReferenceDataService(_apiClient.Object, _mapper.Object, _cacheProvider.Object);
        }

        [Test]
        public async Task ThenCheckSortPriorityIsApplied()
        {
            const string searchTerm = "Bob";

            //Arrange
            _apiClient.Setup(x => x.SearchOrganisations(searchTerm, 500))
                .ReturnsAsync(
                    ConstructOrganisationSearchResults());

            var searchResults = await _referenceDataService.SearchOrganisations(searchTerm);

            // ASSERT ORDER
            var i = 0;
            Assert.AreEqual("BOB", searchResults.Data.ToList()[i].Name);
            i++;
            Assert.AreEqual("BOB LIMITED", searchResults.Data.ToList()[i].Name);
            i++;
            Assert.AreEqual("BOB LTD", searchResults.Data.ToList()[i].Name);
            i++;
            Assert.AreEqual("BOB LTD.", searchResults.Data.ToList()[i].Name);
            i++;
            Assert.AreEqual("BOB PLC", searchResults.Data.ToList()[i].Name);
            i++;
            Assert.AreEqual("BOB PLC.", searchResults.Data.ToList()[i].Name);
            i++;
            Assert.AreEqual("BOB CARTER MEMORIAL YOUTH AND LEISURE CENTRE TRUST", searchResults.Data.ToList()[i].Name);
            i++;
            Assert.AreEqual("Bobbing Village School", searchResults.Data.ToList()[i].Name);
            i++;
            Assert.AreEqual("Bobbing Village School", searchResults.Data.ToList()[i].Name);
            i++;
            Assert.AreEqual("Bobby Moore Academy", searchResults.Data.ToList()[i].Name);
            i++;
            Assert.AreEqual("Bobby Moore School", searchResults.Data.ToList()[i].Name);

            i++;
            Assert.AreEqual("Ling Bob Junior, Infant and Nursery School", searchResults.Data.ToList()[i].Name);
            i++;
            Assert.AreEqual("Ling Bob Nursery School", searchResults.Data.ToList()[i].Name);
            i++;
            Assert.AreEqual("Bnos Zion of Bobov", searchResults.Data.ToList()[i].Name);
            i++;
            Assert.AreEqual("Talmud Torah Bobov Primary School", searchResults.Data.ToList()[i].Name);

            i++;
            Assert.AreEqual("SHOULD BE AT END", searchResults.Data.ToList()[i].Name);
        }

        private static ReferenceData.Api.Client.Dto.Address ConstructStandardAddressDto()
        {
            return new ReferenceData.Api.Client.Dto.Address
            {
                Line1 = "test 1",
                Line2 = "test 2",
                Line3 = "test 3",
                Line4 = "test 4",
                Line5 = "test 5",
                Postcode = "Test code"
            };
        }

        private static IEnumerable<Organisation> ConstructOrganisationSearchResults()
        {
            return new List<Organisation>
            {
                new Organisation
                {
                    Address = ConstructStandardAddressDto(),
                    Code = null,
                    Name = "Talmud Torah Bobov Primary School",
                    RegistrationDate = null,
                    Sector = "Other independent school",
                    SubType = ReferenceData.Api.Client.Dto.OrganisationSubType.None,
                    Type = ReferenceData.Api.Client.Dto.OrganisationType.EducationOrganisation
                },
                new Organisation
                {
                    Address = ConstructStandardAddressDto(),
                    Code = null,
                    Name = "Ling Bob Junior, Infant and Nursery School",
                    RegistrationDate = null,
                    Sector = "Community school",
                    SubType = ReferenceData.Api.Client.Dto.OrganisationSubType.None,
                    Type = ReferenceData.Api.Client.Dto.OrganisationType.EducationOrganisation
                },
                new Organisation
                {
                    Address = ConstructStandardAddressDto(),
                    Code = null,
                    Name = "Bobbing Village School",
                    RegistrationDate = null,
                    Sector = "Community school",
                    SubType = ReferenceData.Api.Client.Dto.OrganisationSubType.None,
                    Type = ReferenceData.Api.Client.Dto.OrganisationType.EducationOrganisation
                },
                new Organisation
                {
                    Address = ConstructStandardAddressDto(),
                    Code = null,
                    Name = "Ling Bob Nursery School",
                    RegistrationDate = null,
                    Sector = "Local authority nursery school",
                    SubType = ReferenceData.Api.Client.Dto.OrganisationSubType.None,
                    Type = ReferenceData.Api.Client.Dto.OrganisationType.EducationOrganisation
                },
                new Organisation
                {
                    Address = ConstructStandardAddressDto(),
                    Code = null,
                    Name = "Bnos Zion of Bobov",
                    RegistrationDate = null,
                    Sector = "Other independent school",
                    SubType = ReferenceData.Api.Client.Dto.OrganisationSubType.None,
                    Type = ReferenceData.Api.Client.Dto.OrganisationType.EducationOrganisation
                },
                new Organisation
                {
                    Address = ConstructStandardAddressDto(),
                    Code = null,
                    Name = "Bobbing Village School",
                    RegistrationDate = null,
                    Sector = "Academy converter",
                    SubType = ReferenceData.Api.Client.Dto.OrganisationSubType.None,
                    Type = ReferenceData.Api.Client.Dto.OrganisationType.EducationOrganisation
                },
                new Organisation
                {
                    Address = ConstructStandardAddressDto(),
                    Code = null,
                    Name = "Bobby Moore School",
                    RegistrationDate = null,
                    Sector = "Free schools",
                    SubType = ReferenceData.Api.Client.Dto.OrganisationSubType.None,
                    Type = ReferenceData.Api.Client.Dto.OrganisationType.EducationOrganisation
                },
                new Organisation
                {
                    Address = ConstructStandardAddressDto(),
                    Code = null,
                    Name = "Bobby Moore Academy",
                    RegistrationDate = null,
                    Sector = "Free schools",
                    SubType = ReferenceData.Api.Client.Dto.OrganisationSubType.None,
                    Type = ReferenceData.Api.Client.Dto.OrganisationType.EducationOrganisation
                },
                new Organisation
                {
                    Address = ConstructStandardAddressDto(),
                    Code = "274110",
                    Name = "BOB CARTER MEMORIAL YOUTH AND LEISURE CENTRE TRUST",
                    RegistrationDate = DateTime.Parse("02/09/1977 00:00:00"),
                    Sector = "Free schools",
                    SubType = ReferenceData.Api.Client.Dto.OrganisationSubType.None,
                    Type = ReferenceData.Api.Client.Dto.OrganisationType.Charity
                },
                new Organisation
                {
                    Address = ConstructStandardAddressDto(),
                    Code = null,
                    Name = "BOB",
                    RegistrationDate = DateTime.Parse("02/09/1977 00:00:00"),
                    Sector = "Free schools",
                    SubType = ReferenceData.Api.Client.Dto.OrganisationSubType.None,
                    Type = ReferenceData.Api.Client.Dto.OrganisationType.Charity
                },
                //THIS MIMICS AN INCORRECT ENTRY BEING RETURNED FROM SERVICE
                new Organisation
                {
                    Address = ConstructStandardAddressDto(),
                    Code = null,
                    Name = "SHOULD BE AT END",
                    RegistrationDate = DateTime.Parse("02/09/1977 00:00:00"),
                    Sector = "Free schools",
                    SubType = ReferenceData.Api.Client.Dto.OrganisationSubType.None,
                    Type = ReferenceData.Api.Client.Dto.OrganisationType.Charity
                },
                new Organisation
                {
                    Address = ConstructStandardAddressDto(),
                    Code = null,
                    Name = "BOB LIMITED",
                    RegistrationDate = DateTime.Parse("02/09/1977 00:00:00"),
                    Sector = "Free schools",
                    SubType = ReferenceData.Api.Client.Dto.OrganisationSubType.None,
                    Type = ReferenceData.Api.Client.Dto.OrganisationType.Charity
                },
                new Organisation
                {
                    Address = ConstructStandardAddressDto(),
                    Code = null,
                    Name = "BOB LTD",
                    RegistrationDate = DateTime.Parse("02/09/1977 00:00:00"),
                    Sector = "Free schools",
                    SubType = ReferenceData.Api.Client.Dto.OrganisationSubType.None,
                    Type = ReferenceData.Api.Client.Dto.OrganisationType.Charity
                },
                new Organisation
                {
                    Address = ConstructStandardAddressDto(),
                    Code = null,
                    Name = "BOB LTD.",
                    RegistrationDate = DateTime.Parse("02/09/1977 00:00:00"),
                    Sector = "Free schools",
                    SubType = ReferenceData.Api.Client.Dto.OrganisationSubType.None,
                    Type = ReferenceData.Api.Client.Dto.OrganisationType.Charity
                },
                new Organisation
                {
                    Address = ConstructStandardAddressDto(),
                    Code = null,
                    Name = "BOB PLC.",
                    RegistrationDate = DateTime.Parse("02/09/1977 00:00:00"),
                    Sector = "Free schools",
                    SubType = ReferenceData.Api.Client.Dto.OrganisationSubType.None,
                    Type = ReferenceData.Api.Client.Dto.OrganisationType.Charity
                },
                new Organisation
                {
                    Address = ConstructStandardAddressDto(),
                    Code = null,
                    Name = "BOB PLC",
                    RegistrationDate = DateTime.Parse("02/09/1977 00:00:00"),
                    Sector = "Free schools",
                    SubType = ReferenceData.Api.Client.Dto.OrganisationSubType.None,
                    Type = ReferenceData.Api.Client.Dto.OrganisationType.Charity
                }

            };
        }
    }
}
