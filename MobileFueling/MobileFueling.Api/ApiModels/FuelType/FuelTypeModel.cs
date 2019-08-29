using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MobileFueling.Api.Common.Localization;
using MobileFueling.Api.Contract.FuelType;
using MobileFueling.DB;
using MobileFueling.ViewModel;
using System.Linq;
using System.Threading.Tasks;

namespace MobileFueling.Api.ApiModels.FuelType
{
    public class FuelTypeModel
    {
        private readonly FuelDbContext _fuelContext;
        private readonly IStringLocalizer _stringLocalizer;

        public FuelTypeModel(FuelDbContext fuelContext, IStringLocalizer stringLocalizer)
        {
            _fuelContext = fuelContext;
            _stringLocalizer = stringLocalizer;
        }

        public async Task<FuelTypeGetAllResponse> GetAll()
        {
            var items = await _fuelContext.FuelTypes.ToListAsync();
            return new FuelTypeGetAllResponse
            {
                Items = items.Select(Convert)
            };
        }

        public async Task<FuelTypeGetOneResponse> GetOne(long id)
        {
            FuelTypeGetOneResponse response = new FuelTypeGetOneResponse();

            var item = await _fuelContext.FuelTypes.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                response.AddMessage(Common.BaseResponseResources.MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.FUEL_TYPE_CAN_NOT_FIND]);
                return response;
            }

            response.Item = Convert(item);
            return response;
        }

        public async Task<FuelTypeUpdateResponse> PostOne(FuelTypeVM fuelTypeVM)
        {
            Model.FuelType fuelType = null;
            if (fuelTypeVM.Id.HasValue)
                fuelType = await _fuelContext.FuelTypes.FirstOrDefaultAsync(x => x.Id == fuelTypeVM.Id.Value);
            else
                fuelType = new Model.FuelType { };

            fuelType.Name = fuelTypeVM.Name;

            if (fuelTypeVM.Id.HasValue)
                _fuelContext.FuelTypes.Update(fuelType);
            else
                await _fuelContext.FuelTypes.AddAsync(fuelType);

            _fuelContext.SaveChanges();

            return new FuelTypeUpdateResponse
            {
                Id = fuelType.Id
            };
        }

        public async Task<FuelTypeDeleteResponse> DeleteOne(long id)
        {
            FuelTypeDeleteResponse response = new FuelTypeDeleteResponse { IsSuccess = false };

            var item = await _fuelContext.FuelTypes.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null)
            {
                response.AddMessage(Common.BaseResponseResources.MessageType.ERROR, _stringLocalizer[CustomStringLocalizer.FUEL_TYPE_CAN_NOT_FIND]);
                return response;
            }

            _fuelContext.FuelTypes.Remove(item);
            await _fuelContext.SaveChangesAsync();

            response.IsSuccess = true;
            return response;
        }

        private FuelTypeVM Convert(Model.FuelType fuelType)
        {
            if (fuelType == null)
                return null;

            return new FuelTypeVM
            {
                Id = fuelType.Id,
                Name = fuelType.Name
            };
        }
    }
}