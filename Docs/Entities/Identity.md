
## ApplicationUser

```csharp
public class ApplicationUser : IdentityUser
{
	// TODO: Add Methods
}
```

```json
{
	"id": "00000000-0000-0000-0000-000000000000",
	"userName": "johndoe",
	"email": "john@email.com",
	"emailConfirmed": false,
	"passwordHash": "",
	// ommited, refer to ASP.NET Identity documentation
}
```

## ApplicationUserRole

```csharp
public class ApplicationUserRole : IdentityRole
{
	// TODO: Add Methods
}
```

```json
{
	"id": "00000000-0000-0000-0000-000000000000",
	"name": "Admin",
	"NormalizedName": "ADMIN",
	"createdAt": "2020-01-01T00:00:00.0000000Z",
	"updatedAt": "2020-01-01T00:00:00.0000000Z",
	"deletedAt": "2020-01-01T00:00:00.0000000Z"
}
```