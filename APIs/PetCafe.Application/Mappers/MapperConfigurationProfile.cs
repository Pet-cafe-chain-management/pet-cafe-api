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

        // Team
        CreateMap<Models.TeamModels.TeamCreateModel, Domain.Entities.Team>().ReverseMap();
        CreateMap<Models.TeamModels.TeamUpdateModel, Domain.Entities.Team>().ReverseMap();

        // Team Member
        CreateMap<Models.TeamModels.MemberCreateModel, Domain.Entities.TeamMember>().ReverseMap();
        CreateMap<Models.TeamModels.MemberUpdateModel, Domain.Entities.TeamMember>().ReverseMap();

        // Work Shift
        CreateMap<Models.WorkShiftModels.WorkShiftCreateModel, Domain.Entities.WorkShift>().ReverseMap();
        CreateMap<Models.WorkShiftModels.WorkShiftUpdateModel, Domain.Entities.WorkShift>().ReverseMap();

        // VaccineType
        CreateMap<Models.VaccineTypeModels.VaccineTypeCreateModel, Domain.Entities.VaccineType>().ReverseMap();
        CreateMap<Models.VaccineTypeModels.VaccineTypeUpdateModel, Domain.Entities.VaccineType>().ReverseMap();

        // PetGroup
        CreateMap<Models.PetGroupModels.PetGroupCreateModel, Domain.Entities.PetGroup>().ReverseMap();
        CreateMap<Models.PetGroupModels.PetGroupUpdateModel, Domain.Entities.PetGroup>().ReverseMap();

        // HealthRecord
        CreateMap<Models.HealthRecordModels.HealthRecordCreateModel, Domain.Entities.HealthRecord>().ReverseMap();
        CreateMap<Models.HealthRecordModels.HealthRecordUpdateModel, Domain.Entities.HealthRecord>().ReverseMap();

        // VaccinationRecord
        CreateMap<Models.VaccinationRecordModels.VaccinationRecordCreateModel, Domain.Entities.VaccinationRecord>().ReverseMap();
        CreateMap<Models.VaccinationRecordModels.VaccinationRecordUpdateModel, Domain.Entities.VaccinationRecord>().ReverseMap();

        // VaccinationSchedule
        CreateMap<Models.VaccinationScheduleModels.VaccinationScheduleCreateModel, Domain.Entities.VaccinationSchedule>().ReverseMap();
        CreateMap<Models.VaccinationScheduleModels.VaccinationScheduleUpdateModel, Domain.Entities.VaccinationSchedule>().ReverseMap();

        // Service

        CreateMap<Models.ServiceModels.ServiceCreateModel, Domain.Entities.Service>().ReverseMap();
        CreateMap<Models.ServiceModels.ServiceUpdateModel, Domain.Entities.Service>().ReverseMap();

        // Slot
        CreateMap<Models.SlotModels.SlotCreateModel, Domain.Entities.Slot>().ReverseMap();
        CreateMap<Models.SlotModels.SlotUpdateModel, Domain.Entities.Slot>().ReverseMap();

        // Order
        CreateMap<Models.OrderModels.OrderCreateModel, Domain.Entities.Order>().ReverseMap();

        // Transaction
        CreateMap<Models.PayOsModels.WebhookData, Domain.Entities.Transaction>().ReverseMap();

        //WorkType

        CreateMap<Models.WorkTypeModels.WorkTypeCreateModel, Domain.Entities.WorkType>().ReverseMap();
        CreateMap<Models.WorkTypeModels.WorkTypeUpdateModel, Domain.Entities.WorkType>().ReverseMap();

    }
}
