<<<<<<< HEAD
﻿using MediatR;
using RoomMateFinder.Features.LikeProfile.LikeRequest;

namespace RoomMateFinder.Features.Matching.LikeProfile;

public record LikeCommand(LikeRequest Request) : IRequest<bool>;
=======
﻿using RoomMateFinder.Features.LikeProfile.LikeRequest;

namespace RoomMateFinder.Features.Matching.LikeProfile;

using MediatR;

public record LikeCommand(Guid UserId, LikeRequest Request) : IRequest<bool>;

>>>>>>> DariusBranch
