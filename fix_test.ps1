$file = "CraftConnect.Tests\UserControllerTests.cs"
$content = Get-Content $file -Raw

# Replace the mock setup with real ClaimsPrincipal
$oldCode = @'
		// Mock User.FindFirst to return the same userId
		var claimsPrincipalMock = new Mock<ClaimsPrincipal>();
		var claim = new Claim(ClaimTypes.NameIdentifier, userId.ToString());
		claimsPrincipalMock.Setup(x => x.FindFirst(ClaimTypes.NameIdentifier)).Returns(claim);
		claimsPrincipalMock.Setup(x => x.IsInRole("Admin")).Returns(false); // Not admin, just regular user
		_usersController.ControllerContext.HttpContext.User = claimsPrincipalMock.Object;
'@

$newCode = @'
		// Use real ClaimsPrincipal (mocking doesn't work with FindFirst)
		var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
		var identity = new ClaimsIdentity(claims, "TestAuth");
		var claimsPrincipal = new ClaimsPrincipal(identity);
		_usersController.ControllerContext.HttpContext.User = claimsPrincipal;
'@

$newContent = $content.Replace($oldCode, $newCode)
Set-Content $file $newContent -NoNewline

Write-Host "Fixed DeleteUserAsync test - replaced Mock with real ClaimsPrincipal"
