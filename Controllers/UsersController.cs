using ApiUser.Models;
using ApiUser.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ApiUser.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UsersService _usersService;

    public UsersController(UsersService usersService) =>
        _usersService = usersService;

    /// <summary>
    /// Devuelve todos los Usuarios
    /// </summary>
    /// <returns></returns>
    [HttpGet]

    public async Task<List<User>> Get() =>
        await _usersService.GetAsync();
    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<User>> Get(string id)
    {
        var user = await _usersService.GetAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        return user;
    }

    /// <summary>
    /// Devuelve el usuario consultado
    /// </summary>
    /// <returns></returns>
    [HttpGet("user")]
    public async Task<ActionResult<User>> GetUser(string user)
    {
        var nameUser = await _usersService.GetUserAsync(user);

        if (nameUser is null)
        {
            return NotFound();
        }

        return nameUser;
    }

    /// <summary>
    /// endPoint para realizar compras
    /// </summary>
    /// <returns></returns>
    [HttpPut("compras")]

    public async Task<IActionResult> UpdateUser(string user, Producto producto)
    {

        var userDb = await _usersService.GetUserAsync(user);
        string hora = DateTime.Now.ToString("hh:mm:ss tt");
        string newMoviment = $"se realizo una compra del producto {producto.nombre} a la hora {hora}";
        string[] arr = new string[] { newMoviment };

        if (userDb is null)
        {
            return NotFound();
        }
        if (producto.precio > userDb.spaceAvailable)
        {
            return NotFound("el precio del producto es mayor al cupo disponible");
        }
        userDb!.spaceAvailable = userDb!.spaceAvailable - producto.precio;
        userDb!.movements = userDb!.movements.Concat(arr).ToArray();

        await _usersService.UpdateUserAsync(user, userDb);

        return Ok(await _usersService.GetUserAsync(user));
    }

    /// <summary>
    /// endpoint para ralizar consignacion
    /// </summary>
    /// <returns></returns>
    [HttpPut("consignacion")]

    public async Task<IActionResult> Consignacion(string user, Transferencia transferencia)
    {
        var userDb = await _usersService.GetUserAsync(user);
        var cuentaDb = await _usersService.GetCountAsync(transferencia.countNumber!);
        if (cuentaDb is null)
        {
            return NotFound("numero de cuenta no existe");
        }
        else if (cuentaDb.documentId != transferencia.documentId)
        {
            return NotFound("transferencia invalida");
        }

        int[] valores = new int[]
        {
            10000, 30000, 50000,
            100000, 200000, 300000, 400000, 500000
        };

        if (valores.Where(x => x == transferencia.valor).FirstOrDefault() == 0)
        {
            return NotFound("valor de transferencia invalido");
        }
        userDb!.spaceAvailable = userDb!.spaceAvailable - transferencia.valor;
        cuentaDb!.spaceAvailable = cuentaDb!.spaceAvailable + transferencia.valor;
        await _usersService.UpdateUserAsync(user, userDb);
        await _usersService.UpdateCountAsync(transferencia.countNumber!, cuentaDb);
        List<User> result = new List<User>
        {
            (await _usersService.GetUserAsync(user))!,
            (await _usersService.GetCountAsync(transferencia.countNumber!))!
        };
        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> Post(User newUser)
    {
        await _usersService.CreateAsync(newUser);

        return CreatedAtAction(nameof(Get), new { id = newUser.userId }, newUser);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, User updatedUser)
    {
        var book = await _usersService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        updatedUser.userId = book.userId;

        await _usersService.UpdateAsync(id, updatedUser);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var book = await _usersService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        await _usersService.RemoveAsync(id);

        return NoContent();
    }


    [HttpGet]
    [Route("load")]
    public async Task<List<User>> Load()
    {
        StreamReader jsonStream = System.IO.File.OpenText(@"users.json");
        var json = jsonStream.ReadToEnd();
        List<User> result = JsonSerializer.Deserialize<List<User>>(json)!;
        foreach (var userArr in result)
        {
            var user = new User()
            {
                name = userArr.name,
                documentId = userArr.documentId,
                countNumber = userArr.countNumber,
                spaceAvailable = userArr.spaceAvailable,
                movements = userArr.movements
            };
            Console.WriteLine(userArr.name);
            await _usersService.CreateAsync(user);
        }
        return await _usersService.GetAsync();
    }
}