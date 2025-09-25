using AutoMapper;

namespace PetCafe.Application.Mappers;

public class MapperConfigurationsProfile : Profile
{
    public MapperConfigurationsProfile()
    {
        // Employee
        CreateMap<Models.EmployeeModels.EmployeeCreateModel, Domain.Entities.Employee>().ReverseMap();
        CreateMap<Models.EmployeeModels.EmployeeUpdateModel, Domain.Entities.Employee>().ReverseMap();

        // Customer
        CreateMap<Models.CustomerModels.CustomerCreateModel, Domain.Entities.Customer>().ReverseMap();
        CreateMap<Models.CustomerModels.CustomerUpdateModel, Domain.Entities.Customer>().ReverseMap();

        // Category
        CreateMap<Models.CategoryModels.CategoryCreateModel, Domain.Entities.ProductCategory>().ReverseMap();
        CreateMap<Models.CategoryModels.CategoryUpdateModel, Domain.Entities.ProductCategory>().ReverseMap();

        //Area
        CreateMap<Models.AreaModels.AreaCreateModel, Domain.Entities.Area>().ReverseMap();
        CreateMap<Models.AreaModels.AreaUpdateModel, Domain.Entities.Area>().ReverseMap();

        // Product
        CreateMap<Models.ProductModels.ProductCreateModel, Domain.Entities.Product>().ReverseMap();
        CreateMap<Models.ProductModels.ProductUpdateModel, Domain.Entities.Product>().ReverseMap();

        //Breed
        CreateMap<Models.BreedModels.BreedCreateModel, Domain.Entities.PetBreed>().ReverseMap();
        CreateMap<Models.BreedModels.BreedUpdateModel, Domain.Entities.PetBreed>().ReverseMap();

        // PetSpecies
        CreateMap<Models.PetSpeciesModels.PetSpeciesCreateModel, Domain.Entities.PetSpecies>().ReverseMap();
        CreateMap<Models.PetSpeciesModels.PetSpeciesUpdateModel, Domain.Entities.PetSpecies>().ReverseMap();

        // Pet
        CreateMap<Models.PetModels.PetCreateModel, Domain.Entities.Pet>().ReverseMap();
        CreateMap<Models.PetModels.PetUpdateModel, Domain.Entities.Pet>().ReverseMap();
    }
}
