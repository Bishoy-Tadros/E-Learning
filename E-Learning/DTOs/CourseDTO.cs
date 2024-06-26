﻿namespace E_Learning.DTOs;

public class CourseDTO
{
    public string CourseId { get; set; }
    public string CourseTile { get; set; }
    
    public string CourseDescription { get; set; }
    
    public double CoursePrice { get; set; }
    
    public string Category { get; set; }
    
    public int Quantity { get; set; }
}