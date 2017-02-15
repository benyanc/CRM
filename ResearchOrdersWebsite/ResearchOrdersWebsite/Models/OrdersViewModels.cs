using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ResearchOrdersWebsite.Models
{
    public enum OrderTypeEnum { SequencingContract }
    public enum ProjectTypeEnum { ByLane, SelfDefined }
    public enum SampleTypeEnum { MixLanesTrue, MixLanesFalse, SelfDefined }
    public enum SequencingPlatformEnum { HiSeq4000, HiSeqXTen, MiSeq }

    public class SequencingContractViewModel:OrderViewModel
    {
        //public void SetBaseInfo(string projectId, DateTime orderSubmissionDate, OrderTypeEnum orderType, string userId, string userName)
        //{
        //    SetBaseInfo(projectId, orderSubmissionDate, orderType, userId, userName);
        //}

        public void SetDetails(
            string organization,
            string orderComments,
            ProjectTypeEnum projectType,
            string numLanes,
            string projectTypeSelfDefinition,
            string sampleGenus,
            string sampleSpecies,
            string geneSize,
            bool splitData,
            string indexDetails,
            SampleTypeEnum sampleType,
            string mixLaneSize,
            string sampleTypeSelfDefinition,
            SequencingPlatformEnum sequencingPlatform,
            bool customerProvidesHardDrive,
            bool sampleAlreadyAtGeneseeq,
            string geneseeqSampleId,
            string deliveryNumber,
            string dataInfoFileName
            )
        {
            Organization = organization;
            OrderComments = orderComments;
            ProjectType = projectType;
            NumLanes = numLanes;
            ProjectTypeSelfDefinition = projectTypeSelfDefinition;
            SampleGenus = sampleGenus;
            SampleSpecies = sampleSpecies;
            GeneSize = geneSize;
            SplitData = splitData;
            IndexDetails = indexDetails;
            SampleType = sampleType;
            MixLaneSize = mixLaneSize;
            SampleTypeSelfDefinition = sampleTypeSelfDefinition;
            SequencingPlatform = sequencingPlatform;
            CustomerProvidesHardDrive = customerProvidesHardDrive;
            SampleAlreadyAtGeneseeq = sampleAlreadyAtGeneseeq;
            GeneseeqSampleId = geneseeqSampleId;
            DeliveryNumber = deliveryNumber;
            DataInfoFileName = dataInfoFileName;
        }

        [Required]
        [Display(Name = "送样单位（请填写公司或单位全称）")]
        public string Organization { get; set; }

        [Display(Name = "订单备注")]
        public string OrderComments { get; set; }

        [Required]
        [Display(Name = "项目类型")]
        public ProjectTypeEnum ProjectType { get; set; }

        [RequiredIf("ProjectType", ProjectTypeEnum.ByLane)]
        [Display(Name = "总数量（条）")]
        public string NumLanes { get; set; }

        [RequiredIf("ProjectType", ProjectTypeEnum.SelfDefined)]
        [Display(Name = "项目自定义")]
        public string ProjectTypeSelfDefinition { get; set; }

        [Display(Name = "属（研究样本）")]
        public string SampleGenus { get; set; }

        [Display(Name = "种（研究样本）")]
        public string SampleSpecies { get; set; }

        [Display(Name = "基因组大小（Mbp)")]
        public string GeneSize { get; set; }

        [Required]
        [Display(Name = "是否需要拆分数据")]
        public bool SplitData { get; set; }

        [RequiredIf("SplitData", true)]
        [Display(Name = "（若需要，请填写 index 的详细信息）")]
        public string IndexDetails { get; set; }

        [Required]
        [Display(Name = "样本类型")]
        public SampleTypeEnum SampleType { get; set; }

        [RequiredIf("SampleType", SampleTypeEnum.MixLanesTrue)]
        [Display(Name = "需要混 lane 的子文库数量")]
        public string MixLaneSize { get; set; }

        [RequiredIf("SampleType", SampleTypeEnum.SelfDefined)]
        [Display(Name = "样本自定义")]
        public string SampleTypeSelfDefinition { get; set; }

        [Required]
        [Display(Name = "测序平台")]
        public SequencingPlatformEnum SequencingPlatform { get; set; }

        [Required]
        [Display(Name = "是否携带硬盘")]
        public bool CustomerProvidesHardDrive { get; set; }

        [Required]
        [Display(Name = "是否样本已送到世和")]
        public bool SampleAlreadyAtGeneseeq { get; set; }

        [RequiredIf("SampleAlreadyAtGeneseeq", true)]
        [Display(Name = "世和样本号")]
        public string GeneseeqSampleId { get; set; }

        [RequiredIf("SampleAlreadyAtGeneseeq", false)]
        [Display(Name = "样本快递单号")]
        public string DeliveryNumber { get; set; }

        [Display(Name = "上传文件名称")]
        public string DataInfoFileName { get; set; }
        //[Required]
        //[Display(Name = "上传文件（最大4MB）")]
        //[DataType(DataType.Upload)]
        //public HttpPostedFileBase DataInfoFile { get; set; }
    }

    public class OrderViewModel
    {
        [Required]
        [Key]
        [Display(Name = "订单名称（项目编号）")]
        public string ProjectId { get; set; }

        [Display(Name = "订单类型")]
        public OrderTypeEnum OrderType { get; set; }

        [Display(Name = "订单提交日期")]
        public DateTime OrderSubmissionDate { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }

        public void SetBaseInfo(string projectId, DateTime orderSubmissionDate, OrderTypeEnum orderType, string userId, string userName)
        {
            ProjectId = projectId;
            OrderSubmissionDate = orderSubmissionDate;
            OrderType = orderType;
            UserId = userId;
            UserName = userName;
        }

    }
}