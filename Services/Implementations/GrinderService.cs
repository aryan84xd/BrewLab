using BrewLab.Authentication;
using BrewLab.Models.Entities;
using BrewLab.Models.RequestModels;
using BrewLab.Models.ResponseModels;
using BrewLab.Repositories.Interfaces;
using BrewLab.Services.Interfaces;

namespace BrewLab.Services.Implementations
{
    public class GrinderService : IGrinderService
    {
        private readonly IGrinderRepository _grinderRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ResponseFactory _responseFactory;

        public GrinderService(
            IGrinderRepository grinderRepository,
            ICurrentUserService currentUserService,
            ResponseFactory responseFactory)
        {
            _grinderRepository = grinderRepository ?? throw new ArgumentNullException(nameof(grinderRepository));
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _responseFactory = responseFactory ?? throw new ArgumentNullException(nameof(responseFactory));
        }

        public async Task<IEnumerable<GrinderResponseModel>> GetAllAsync()
        {
            var userId = _currentUserService.UserId;

            var grinders = await _grinderRepository.GetAllAsync(userId);

            return grinders.Select(MapToResponseModel);
        }

        public async Task<GrinderResponseModel> GetByIdAsync(Guid grinderId)
        {
            if (grinderId == Guid.Empty)
            {
                return _responseFactory.Failure<GrinderResponseModel>(
                    "InvalidGrinderId",
                    "Grinder ID is required");
            }

            var grinder = await _grinderRepository.GetByIdAsync(grinderId);

            if (grinder == null)
            {
                return _responseFactory.Failure<GrinderResponseModel>(
                    "GrinderNotFound",
                    "Grinder not found");
            }

            var userId = _currentUserService.UserId;

            if (grinder.UserId != userId)
            {
                return _responseFactory.Failure<GrinderResponseModel>(
                    "Unauthorized",
                    "You do not have access to this grinder");
            }

            return MapToResponseModel(grinder);
        }

        public async Task<GrinderResponseModel> CreateAsync(CreateGrinderRequestModel request)
        {
            if (request == null)
            {
                return _responseFactory.Failure<GrinderResponseModel>(
                    "InvalidRequest",
                    "Grinder request cannot be null");
            }

            try
            {
                var grinder = new Grinder
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    UserId = _currentUserService.UserId
                };

                await _grinderRepository.CreateAsync(grinder);

                return MapToResponseModel(grinder);
            }
            catch (Exception ex)
            {
                return _responseFactory.Failure<GrinderResponseModel>(
                    "CreationFailed",
                    $"An error occurred while creating the grinder: {ex.Message}");
            }
        }

        public async Task<GrinderResponseModel> UpdateAsync(
            Guid grinderId,
            UpdateGrinderRequestModel request)
        {
            if (grinderId == Guid.Empty)
            {
                return _responseFactory.Failure<GrinderResponseModel>(
                    "InvalidGrinderId",
                    "Grinder ID is required");
            }

            if (request == null)
            {
                return _responseFactory.Failure<GrinderResponseModel>(
                    "InvalidRequest",
                    "Grinder request cannot be null");
            }

            try
            {
                var userId = _currentUserService.UserId;

                var exists = await _grinderRepository.ExistsAsync(grinderId, userId);

                if (!exists)
                {
                    return _responseFactory.Failure<GrinderResponseModel>(
                        "Unauthorized",
                        "You do not have access to this grinder");
                }

                var grinder = await _grinderRepository.GetByIdAsync(grinderId);

                if (grinder == null)
                {
                    return _responseFactory.Failure<GrinderResponseModel>(
                        "GrinderNotFound",
                        "Grinder not found");
                }

                grinder.Name = request.Name;

                await _grinderRepository.UpdateAsync(grinder);

                return MapToResponseModel(grinder);
            }
            catch (Exception ex)
            {
                return _responseFactory.Failure<GrinderResponseModel>(
                    "UpdateFailed",
                    $"An error occurred while updating the grinder: {ex.Message}");
            }
        }

        public async Task<GrinderResponseModel> DeleteAsync(Guid grinderId)
        {
            if (grinderId == Guid.Empty)
            {
                return _responseFactory.Failure<GrinderResponseModel>(
                    "InvalidGrinderId",
                    "Grinder ID is required");
            }

            try
            {
                var userId = _currentUserService.UserId;

                var exists = await _grinderRepository.ExistsAsync(grinderId, userId);

                if (!exists)
                {
                    return _responseFactory.Failure<GrinderResponseModel>(
                        "Unauthorized",
                        "You do not have access to this grinder");
                }

                var grinder = await _grinderRepository.GetByIdAsync(grinderId);

                if (grinder == null)
                {
                    return _responseFactory.Failure<GrinderResponseModel>(
                        "GrinderNotFound",
                        "Grinder not found");
                }

                await _grinderRepository.DeleteAsync(grinder);

                return new GrinderResponseModel
                {
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return _responseFactory.Failure<GrinderResponseModel>(
                    "DeleteFailed",
                    $"An error occurred while deleting the grinder: {ex.Message}");
            }
        }

        private static GrinderResponseModel MapToResponseModel(Grinder grinder)
        {
            return new GrinderResponseModel
            {
                Success = true,

                Id = grinder.Id,
                Name = grinder.Name
            };
        }

        Task<BaseResponse> IGrinderService.DeleteAsync(Guid grinderId)
        {
            throw new NotImplementedException();
        }
    }
}