using System;
using System.ComponentModel.DataAnnotations;

namespace SpecSelRepos.Models
{
    public class SpecSelResult
    {
        public int ID { get; set; }

        [StringLength(60, MinimumLength = 3), Display(Name = "Data Set"), Required]
        public string DataSet { get; set; }

        [Display(Name = "No. Species"), Range(1, int.MaxValue), RegularExpression(@"^[1-9]\d*$")]
        public int NumSpecies { get; set; }

        [Display(Name = "No. Resources"), Range(1, int.MaxValue), RegularExpression(@"^[1-9]\d*$")]
        public int NumResources { get; set; }

        [DataType(DataType.Text), StringLength(3, MinimumLength = 1),
         RegularExpression(@"^(A|B|BN|BF|BFN|C|CF|CN|CFN)$", ErrorMessage = "Must be one of: A, B, BN, BF, BFN, C, CF, CN, CFN"), Required]
        public string Option { get; set; }

        [Display(Name = "M"), Range(0, int.MaxValue), RegularExpression(@"^[0-9]\d*$")]
        public int SpeciesThresholdM { get; set; }

        [Display(Name = "X"), Range(0.0, Double.MaxValue)]
        public decimal SdThresholdX { get; set; }

        [Display(Name = "Y"), Range(0.0, Double.MaxValue)]
        public decimal AreaPrecisionThresholdY { get; set; }

        [DataType(DataType.MultilineText), Required]
        public string Output { get; set; }
    }
}
