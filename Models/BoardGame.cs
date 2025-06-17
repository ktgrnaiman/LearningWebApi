﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Learning.Models;

[Table("BoardGames")]
public class BoardGame
{
    [Key]
    public int Id { get; set; }

    [Required] [MaxLength(200)] 
    public string Name { get; set; } = null!;
        
    [Required]
    public int Year { get; set; }
    
    [Required]
    public int PublisherId { get; set; }
    
    [Required]
    public int MinPlayers { get; set; }
        
    [Required]
    public int MaxPlayers { get; set; }
        
    [Required]
    public int PlayTime { get; set; }
        
    [Required]
    public int MinAge { get; set; }
        
    [Required]
    public int UsersRated { get; set; }
        
    [Required] [Precision(4,2)]
    public decimal RatingAverage { get; set; }
        
    [Required]
    public int BGGRank { get; set; }
        
    [Required] [Precision(4, 2)]
    public decimal ComplexityAverage { get; set; }
        
    [Required]
    public int OwnedUsers { get; set; }
        
    [Required]
    public DateTime CreatedDate { get; set; }
        
    [Required]
    public DateTime LastModifiedDate { get; set; }
    
    [MaxLength(200)] 
    public string AlternateNames { get; set; }
    
    [MaxLength(200)]
    public string Designer { get; set; }
    
    [Required]
    public int Flags { get; set; }
    
    public Publisher? Publisher { get; set; }
    
    public ICollection<BoardGame_Domain>? DomainsJunction { get; set; }
    
    public ICollection<BoardGame_Mechanic>? MechanicsJunction { get; set; }
    
    public ICollection<BoardGame_Category>? CategoriesJunction { get; set; }
}
