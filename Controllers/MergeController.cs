using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MergeApi.Data;
using MergeApi.Models;
using System.Text.Json;

namespace MergeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MergeController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly bool _checkSorted;
    private readonly int _maxArrayLength;

    public MergeController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _checkSorted = Environment.GetEnvironmentVariable("CHECK_SORTED") is string cs
            ? bool.Parse(cs)
            : true;
        _maxArrayLength = Environment.GetEnvironmentVariable("MAX_ARRAY_LENGTH") is string ml
            ? int.Parse(ml)
            : 10000;
    }

    [HttpPost]
    public async Task<IActionResult> MergeArrays([FromBody] MergeRequest request)
    {
        if (request.Array1 == null || request.Array2 == null)
            return BadRequest("Both arrays must be provided.");
        try{
        
        if (request.Array1.Count > _maxArrayLength || request.Array2.Count > _maxArrayLength)
            return BadRequest($"Each array must not exceed {_maxArrayLength} elements.");

        List<int> result;
        
        if (_checkSorted)
        {
            if (!request.Array1.SequenceEqual(request.Array1.OrderBy(x => x)) ||
                !request.Array2.SequenceEqual(request.Array2.OrderBy(x => x)))
            {
                return BadRequest("Both arrays must be sorted in ascending order");
            }

            result = MergeSortedArrays(request.Array1, request.Array2);
        }
        
        else
        {
            // Merge + sort
            result = request.Array1.Concat(request.Array2).OrderBy(x => x).ToList();
        }

        var record = new MergeRecord
        {
            Array1 = JsonSerializer.Serialize(request.Array1),
            Array2 = JsonSerializer.Serialize(request.Array2),
            Result = JsonSerializer.Serialize(result),
            Length = result.Count,
            Timestamp = DateTime.UtcNow
        };

        _context.Merges.Add(record);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Result = result,
            Length = record.Length,
            Timestamp = record.Timestamp
        });
        }
        catch(Exception ex)
        {
            return StatusCode(500, "An error occurred while processing the request: " + ex.Message);
        }
    }
    
    [HttpGet("by-length/{length:int}")]
    public async Task<IActionResult> GetByLength(int length)
    {
        var matches = await _context.Merges
            .Where(r => r.Length == length)
            .ToListAsync();
        
        // Deserialize arrays before returning
        var response = matches.Select(r => new
        {
            Array1 = JsonSerializer.Deserialize<List<int>>(r.Array1),
            Array2 = JsonSerializer.Deserialize<List<int>>(r.Array2),
            Result = JsonSerializer.Deserialize<List<int>>(r.Result),
            Length = r.Length,
            Timestamp = r.Timestamp
        });

        return Ok(response);
    }

    // For O(n) merge of two sorted arrays
    private static List<int> MergeSortedArrays(List<int> a, List<int> b)
    {
        var result = new List<int>(a.Count + b.Count);
        int i = 0, j = 0;

        while (i < a.Count && j < b.Count)
        {
            if (a[i] <= b[j])
                result.Add(a[i++]);
            else
                result.Add(b[j++]);
        }

        while (i < a.Count)
            result.Add(a[i++]);

        while (j < b.Count)
            result.Add(b[j++]);

        return result;
    }

}
