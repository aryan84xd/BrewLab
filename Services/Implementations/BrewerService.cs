using BrewLab.Authentication;
using BrewLab.Models.Entities;
using BrewLab.Models.RequestModels;
using BrewLab.Models.ResponseModels;
using BrewLab.Repositories.Interfaces;
using BrewLab.Services.Interfaces;

namespace BrewLab.Services.Implementations
{
    public class BrewerService : IBrewerService
    {
        private readonly IBrewerRepository _brewerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ResponseFactory _responseFactory;

        public BrewerService(
            IBrewerRepository brewerRepository,
            ICurrentUserService currentUserService,
            ResponseFactory responseFactory)
        {
            _brewerRepository = brewerRepository ?? throw new ArgumentNullException(nameof(brewerRepository));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _responseFactory = responseFactory ?? throw new ArgumentNullException(nameof(responseFactory));
        }

        public async Task<IEnumerable<BrewerResponseModel>> GetAllAsync()
        {
            var userId = _currentUserService.UserId;

            var brewers = await _brewerRepository.GetAllAsync(userId);

            return brewers.Select(MapToResponseModel);
        }

        public async Task<BrewerResponseModel> GetByIdAsync(Guid brewerId)
        {
            if (brewerId == Guid.Empty)
            {
                return _responseFactory.Failure<BrewerResponseModel>(
                    "InvalidBrewerId",
                    "Brewer ID is required");
            }

            var brewer = await _brewerRepository.GetByIdAsync(brewerId);

            if (brewer == null)
            {
                return _responseFactory.Failure<BrewerResponseModel>(
                    "BrewerNotFound",
                    "Brewer not found");
            }

            var userId = _currentUserService.UserId;

            if (brewer.UserId != userId)
            {
                return _responseFactory.Failure<BrewerResponseModel>(
                    "Unauthorized",
                    "You do not have access to this brewer");
            }

            return MapToResponseModel(brewer);
        }

        public async Task<BrewerResponseModel> CreateAsync(CreateBrewerRequestModel request)
        {
            if (request == null)
            {
                return _responseFactory.Failure<BrewerResponseModel>(
                    "InvalidRequest",
                    "Brewer request cannot be null");
            }

            if (request.BrewMethodId == Guid.Empty)
            {
                return _responseFactory.Failure<BrewerResponseModel>(
                    "InvalidBrewMethodId",
                    "Brew Method ID is required");
            }

            try
            {
                var brewer = new Brewer
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    BrewMethodId = request.BrewMethodId,
                    UserId = _currentUserService.UserId
                };

                await _brewerRepository.CreateAsync(brewer);

                return MapToResponseModel(brewer);
            }
            catch (Exception ex)
            {
                return _responseFactory.Failure<BrewerResponseModel>(
                    "CreationFailed",
                    $"An error occurred while creating the brewer: {ex.Message}");
            }
        }

        public async Task<BrewerResponseModel> UpdateAsync(
            Guid brewerId,
            UpdateBrewerRequestModel request)
        {
            if (brewerId == Guid.Empty)
            {
                return _responseFactory.Failure<BrewerResponseModel>(
                    "InvalidBrewerId",
                    "Brewer ID is required");
            }

            if (request == null)
            {
                return _responseFactory.Failure<BrewerResponseModel>(
                    "InvalidRequest",
                    "Brewer request cannot be null");
            }

            try
            {
                var userId = _currentUserService.UserId;

                var exists = await _brewerRepository.ExistsAsync(brewerId, userId);

                if (!exists)
                {
                    return _responseFactory.Failure<BrewerResponseModel>(
                        "Unauthorized",
                        "You do not have access to this brewer");
                }

                var brewer = await _brewerRepository.GetByIdAsync(brewerId);

                if (brewer == null)
                {
                    return _responseFactory.Failure<BrewerResponseModel>(
                        "BrewerNotFound",
                        "Brewer not found");
                }

                brewer.Name = request.Name;
                brewer.BrewMethodId = request.BrewMethodId;

                await _brewerRepository.UpdateAsync(brewer);

                return MapToResponseModel(brewer);
            }
            catch (Exception ex)
            {
                return _responseFactory.Failure<BrewerResponseModel>(
                    "UpdateFailed",
                    $"An error occurred while updating the brewer: {ex.Message}");
            }
        }

        public async Task<BrewerResponseModel> DeleteAsync(Guid brewerId)
        {
            if (brewerId == Guid.Empty)
            {
                return _responseFactory.Failure<BrewerResponseModel>(
                    "InvalidBrewerId",
                    "Brewer ID is required");
            }

            try
            {
                var userId = _currentUserService.UserId;

                var exists = await _brewerRepository.ExistsAsync(brewerId, userId);

                if (!exists)
                {
                    return _responseFactory.Failure<BrewerResponseModel>(
                        "Unauthorized",
                        "You do not have access to this brewer");
                }

                var brewer = await _brewerRepository.GetByIdAsync(brewerId);

                if (brewer == null)
                {
                    return _responseFactory.Failure<BrewerResponseModel>(
                        "BrewerNotFound",
                        "Brewer not found");
                }

                await _brewerRepository.DeleteAsync(brewer);

                return new BrewerResponseModel
                {
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return _responseFactory.Failure<BrewerResponseModel>(
                    "DeleteFailed",
                    $"An error occurred while deleting the brewer: {ex.Message}");
            }
        }

        private static BrewerResponseModel MapToResponseModel(Brewer brewer)
        {
            return new BrewerResponseModel
            {
                Success = true,

                Id = brewer.Id,
                Name = brewer.Name,
                BrewMethodId = brewer.BrewMethodId
            };
        }

        Task<BaseResponse> IBrewerService.DeleteAsync(Guid brewerId)
        {
            throw new NotImplementedException();
        }
    }
}