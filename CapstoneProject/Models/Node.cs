/*
 * Created by Levi Delezene 
 */
 
 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//added namespace-alankar
namespace CapstoneProject.Models
{
    public class Node
    {
        public float xPos { get; set; }
        public float yPos { get; set; }
        public Task Task { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
        //public float[] InConnection { get; set; } //TODO: need to think more carefully about these
        //public float[] OutConnection { get; set; } //TODO: need to think more carefully about these
    }
}