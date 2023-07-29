using AutoMapper;
using TransactionReportingAPI.Models;
using TransactionReportingAPI.Models.DTOs;

namespace TransactionReportingAPI.Data
{
    public class MappingConfig: Profile
    {
        public MappingConfig()
        {
            CreateMap<Transaction, TransactionPostingDetails>().ReverseMap();
        }
    }
}
