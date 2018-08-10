using AlbiruniML;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alb = AlbiruniML.Ops;
namespace TinyYolo
{
    public class TinyYoloPredictor
    {
        Dictionary<string, Tensor> variables = new Dictionary<string, Tensor>();

        private string[] class_names = new string[]{ "person",
          "bicycle",
          "car",
          "motorbike",
          "aeroplane",
          "bus",
          "train",
          "truck",
          "boat",
          "traffic light",
          "fire hydrant",
          "stop sign",
          "parking meter",
          "bench",
          "bird",
          "cat",
          "dog",
          "horse",
          "sheep",
          "cow",
          "elephant",
          "bear",
          "zebra",
          "giraffe",
          "backpack",
          "umbrella",
          "handbag",
          "tie",
          "suitcase",
          "frisbee",
          "skis",
          "snowboard",
          "sports ball",
          "kite",
          "baseball bat",
          "baseball glove",
          "skateboard",
          "surfboard",
          "tennis racket",
          "bottle",
          "wine glass",
          "cup",
          "fork",
          "knife",
          "spoon",
          "bowl",
          "banana",
          "apple",
          "sandwich",
          "orange",
          "broccoli",
          "carrot",
          "hot dog",
          "pizza",
          "donut",
          "cake",
          "chair",
          "sofa",
          "pottedplant",
          "bed",
          "diningtable",
          "toilet",
          "tvmonitor",
          "laptop",
          "mouse",
          "remote",
          "keyboard",
          "cell phone",
          "microwave",
          "oven",
          "toaster",
          "sink",
          "refrigerator",
          "book",
          "clock",
          "vase",
          "scissors",
          "teddy bear",
          "hair drier",
        "toothbrush"};

        private static float[] ReadW(string filename)
        {
            var d = File.ReadAllBytes(filename);

            float[] s = new float[d.Length / 4];
            int c = 0;
            for (int i = 0; i < d.Length; i += 4)
            {
                byte[] num = new byte[4];
                num[0] = d[i];
                num[1] = d[i + 1];
                num[2] = d[i + 2];
                num[3] = d[i + 3];
                s[c] = ToFloat(num);
                c++;
            }
            return s;
        }

        static float ToFloat(byte[] input)
        {
            byte[] newArray = new[] { input[0], input[1], input[2], input[3] };
            return BitConverter.ToSingle(newArray, 0);
        }

        public Dictionary<string, Tensor> LoadFolder(string folderName)
        {
            Dictionary<string, Shard> obj =
                JsonConvert.DeserializeObject<Dictionary<string, Shard>>(File.ReadAllText(folderName + "/manifest.json"));
            Dictionary<string, Tensor> res = new Dictionary<string, Tensor>();
            foreach (var item in obj)
            {
                List<float> ww = new List<float>();
                for (int i = 0; i < item.Value.paths.Length; i++)
                {
                    float[] sh = ReadW(folderName + "/" + item.Value.paths[i]);
                    ww.AddRange(sh);
                }
                var s = ww.ToArray();
                int pointer = 0;
                foreach (var we in item.Value.weights)
                {
                    int size = 1;
                    for (int i = 0; i < we.shape.Length; i++)
                    {
                        size *= we.shape[i];
                    }
                    float[] data = new float[size];
                    for (int i = 0; i < size; i++)
                    {

                        data[i] = s[pointer];
                        pointer++;
                    }
                    var t = alb.tensor(data, we.shape);
                    res.Add(we.name, t);
                    alb.keep(t);

                }
            }
            this.variables = res;
            return res;
        }
        public delegate void ReportProgressHandler(int progress);
        public event ReportProgressHandler ReportProgress;
     
        public Tensor Activations(Tensor input)
        {
            var conv1 = this.convBlock(input, new int[] { 1, 1 }, 1, true, new int[] { 2, 2 });
            InvokeProgressEvent(10);
            var conv2 = this.convBlock(conv1, new int[] { 1, 1 }, 2, true, new int[] { 2, 2 });
            InvokeProgressEvent(20);
            var conv3 = this.convBlock(conv2, new int[] { 1, 1 }, 3, true, new int[] { 2, 2 });
            InvokeProgressEvent(30);
            var conv4 = this.convBlock(conv3, new int[] { 1, 1 }, 4, true, new int[] { 2, 2 });
            InvokeProgressEvent(40);
            var conv5 = this.convBlock(conv4, new int[] { 1, 1 }, 5, true, new int[] { 2, 2 });
            InvokeProgressEvent(50);
            var conv6 = this.convBlock(conv5, new int[] { 1, 1 }, 6, true, new int[] { 1, 1 });
            InvokeProgressEvent(60);
            var conv7 = this.convBlock(conv6, new int[] { 1, 1 }, 7);
            InvokeProgressEvent(70);
            var conv8 = this.convBlock(conv7, new int[] { 1, 1 }, 8);
            InvokeProgressEvent(80);
            var x1 = conv8.conv2d(
             variables["conv2d_9/kernel"], new int[] { 1, 1 }, PadType.same, new int[] { 1, 1 })
             .add(variables["conv2d_9/bias"]);
            InvokeProgressEvent(90);
            return x1;
        }
        private void InvokeProgressEvent(int progress)
        {
            if (this.ReportProgress!=null)
            {
                this.ReportProgress(progress);
            }
        }

        private Tensor convBlock(Tensor inputs, int[] strides, int block,
            bool pool = false,
            int[] poolstrides = null)
        {
            
            var x1 = inputs.conv2d(
        variables["conv2d_" + block.ToString() + "/kernel"], strides, PadType.same, new int[] { 1, 1 });
           
            var x2 = x1.batchNormalization(
                 variables["batch_normalization_" + block.ToString() + "/moving_mean"],
                 variables["batch_normalization_" + block.ToString() + "/moving_variance"], .001f,
                 variables["batch_normalization_" + block.ToString() + "/gamma"],
                 variables["batch_normalization_" + block.ToString() + "/beta"]);
            

            if (pool)
            {  
                var xp = x2.maxPool(new int[] { 2, 2 }, poolstrides, PadType.same);
                 
                
                return xp.leakyRelu(0.10000000149011612f);
            }
            else
            {  
                var res = x2.leakyRelu(0.10000000149011612f);
             
                return res;
            }


        }



        private Tensor YOLO_ANCHORS;
        public TinyYoloPredictor()
        {
            this.LoadFolder("data");
            this.YOLO_ANCHORS = alb.tensor2d(new float[10]{
            0.57273f, 0.677385f, 1.87446f, 2.06253f, 3.33843f, 5.47434f, 7.88282f, 3.52778f,
    9.77052f, 9.16828f
            }, 5, 2);

        }
        public Tuple<Tensor, Tensor, Tensor> yolo_filter_boxes(Tensor boxes, Tensor box_confidence, Tensor box_class_probs, float threshold)
        {
            var box_scores = alb.mul(box_confidence, box_class_probs);
            var box_classes = alb.argMax(box_scores, new int[] { -1 });
            var box_class_scores = alb.max(box_scores, new int[] { -1 });
            var prediction_mask = alb.greaterEqual(box_class_scores, alb.scalar(threshold)).as1D();
            var N = prediction_mask.Size;
            // linspace start/stop is inclusive.
            var all_indices = alb.linspace(0, N - 1, N);
            var neg_indices = alb.zeros(new int[] { N });
            var indices = alb.where(prediction_mask, all_indices, neg_indices);

            var t1 = alb.gather(boxes.reshape(new int[] { N, 4 }), indices);
            var t2 = alb.gather(box_class_scores.flatten(), indices);
            var t3 = alb.gather(box_classes.flatten(), indices);

            return new Tuple<Tensor, Tensor, Tensor>(t1, t2, t3);
        }

        /// <summary>
        ///  Given XY and WH tensor outputs of yolo_head, returns corner coordinates.
        /// </summary>
        /// <param name="box_xy">Bounding box center XY coordinate Tensor</param>
        /// <param name="box_wh">Bounding box WH Tensor</param>
        /// <returns>Bounding box corner Tensor</returns>
        public Tensor yolo_boxes_to_corners(Tensor box_xy, Tensor box_wh)
        {
            var two = alb.scalar(2.0f);
            var box_mins = alb.sub(box_xy, alb.div(box_wh, two));
            var box_maxes = alb.add(box_xy, alb.div(box_wh, two));

            var dim_0 = box_mins.Shape[0];
            var dim_1 = box_mins.Shape[1];
            var dim_2 = box_mins.Shape[2];
            var size = new int[] { dim_0, dim_1, dim_2, 1 };

            return alb.concat(new Tensor[]{
                box_mins.slice( new int[]{0, 0, 0, 1}, size),
                box_mins.slice( new int[]{0, 0, 0, 0}, size),
                box_maxes.slice( new int[]{0, 0, 0, 1}, size),
                box_maxes.slice( new int[]{0, 0, 0, 0}, size),
              }, 3);
        }



        public List<BoundingBox> non_max_suppression(float[] boxes, float[] scores, float iouThreshold)
        {
            // Zip together scores, box corners, and index
            var zipped = new List<BoundingBox>();
            for (var i = 0; i < scores.Length; i++)
            {
                zipped.Add(new BoundingBox()
                {
                    score =
                        scores[i],
                    axis = new float[] { boxes[4 * i], boxes[4 * i + 1], boxes[4 * i + 2], boxes[4 * i + 3] },
                    index = i,
                });
            }


            // Sort by descending order of scores (first index of zipped array)
            var sorted_boxes = zipped.OrderByDescending(p => p.score).ToList();

            var selected_boxes = new List<BoundingBox>();
            // Greedily go through boxes in descending score order and only
            // return boxes that are below the IoU threshold.
            foreach (var box in sorted_boxes)
            {
                var add = true;
                for (var i = 0; i < selected_boxes.Count; i++)
                {
                    // Compare IoU of zipped[1], since that is the box coordinates arr
                    // TODO: I think there"s a bug in this calculation
                    var cur_iou = box_iou(box.axis, selected_boxes[i].axis);
                    if (cur_iou > iouThreshold)
                    {
                        add = false;
                        break;
                    }
                }
                if (add)
                {
                    selected_boxes.Add(box);
                }
            }

            return selected_boxes;

        }

        public Tensor[] yolo_head(Tensor feats, Tensor anchors, int num_classes)
        {
            var num_anchors = anchors.Shape[0];

            var anchors_tensor = alb.reshape(anchors, new int[] { 1, 1, num_anchors, 2 });
            var conv_dims = new int[] { feats.Shape[1], feats.Shape[2] };

            // For later use
            var conv_dims_0 = conv_dims[0];
            var conv_dims_1 = conv_dims[1];

            var conv_height_index = alb.range(0, conv_dims[0]);
            var conv_width_index = alb.range(0, conv_dims[1]);
            conv_height_index = alb.tile(conv_height_index, new int[] { conv_dims[1] });

            conv_width_index = alb.tile(alb.expandDims(conv_width_index, 0), new int[] { conv_dims[0], 1 });
            conv_width_index = alb.transpose(conv_width_index).flatten();

            var conv_index = alb.transpose(alb.stack(new Tensor[] { conv_height_index, conv_width_index }));
            conv_index = alb.reshape(conv_index, new int[] { conv_dims[0], conv_dims[1], 1, 2 });

            feats = alb.reshape(feats, new int[] { conv_dims[0], conv_dims[1], num_anchors, num_classes + 5 });
            float[] conv_dimsInt = new float[conv_dims.Length];
            for (int i = 0; i < conv_dims.Length; i++)
            {
                conv_dimsInt[i] = (float)conv_dims[i];
            }
            var conv_dims2 = alb.reshape(alb.tensor1d(conv_dimsInt), new int[] { 1, 1, 1, 2 });

            var box_xy = alb.sigmoid(feats.slice(new int[] { 0, 0, 0, 0 }, new int[] { conv_dims_0, conv_dims_1, num_anchors, 2 }));
            var box_wh = alb.exp(feats.slice(new int[] { 0, 0, 0, 2 }, new int[] { conv_dims_0, conv_dims_1, num_anchors, 2 }));
            var box_confidence = alb.sigmoid(feats.slice(new int[] { 0, 0, 0, 4 }, new int[] { conv_dims_0, conv_dims_1, num_anchors, 1 }));
            var box_class_probs = alb.softmax(feats.slice(new int[] { 0, 0, 0, 5 }, new int[] { conv_dims_0, conv_dims_1, num_anchors, num_classes }));

            box_xy = alb.div(alb.add(box_xy, conv_index), conv_dims2);
            box_wh = alb.div(alb.mul(box_wh, anchors_tensor), conv_dims2);

            return new Tensor[] { box_xy, box_wh, box_confidence, box_class_probs };
        }

        public float box_intersection(float[] a, float[] b)
        {
            var w = Math.Min(a[3], b[3]) - Math.Max(a[1], b[1]);
            var h = Math.Min(a[2], b[2]) - Math.Max(a[0], b[0]);
            if (w < 0 || h < 0)
            {
                return 0;
            }
            return w * h;
        }

        public float box_union(float[] a, float[] b)
        {
            var i = box_intersection(a, b);
            return (a[3] - a[1]) * (a[2] - a[0]) + (b[3] - b[1]) * (b[2] - b[0]) - i;
        }
        public float box_iou(float[] a, float[] b)
        {
            return box_intersection(a, b) / box_union(a, b);
        }




        int INPUT_DIM = 416;
        float DEFAULT_FILTER_BOXES_THRESHOLD = 0.01f;
        float DEFAULT_IOU_THRESHOLD = 0.4f;
        float DEFAULT_CLASS_PROB_THRESHOLD = 0.4f;

        public List<YoloBox> Detect(Tensor input)
        {

            var activation = this.Activations(input);

            var heads = yolo_head(activation, YOLO_ANCHORS, 80);

            var box_xy = heads[0];
            var box_wh = heads[1];
            var box_confidence = heads[2];
            var box_class_probs = heads[3];

            var all_boxes = yolo_boxes_to_corners(box_xy, box_wh);
            var dd = yolo_filter_boxes(
  all_boxes, box_confidence, box_class_probs, DEFAULT_FILTER_BOXES_THRESHOLD);

            var boxes = dd.Item1;
            var scores = dd.Item2;
            var classes = dd.Item3;
            if (boxes == null)
            {
                return new List<YoloBox>();
            }
            var width = alb.scalar(INPUT_DIM);
            var height = alb.scalar(INPUT_DIM);
            var image_dims = alb.stack(new Tensor[] { height, width, height, width }).reshape(new int[] { 1, 4 });
            boxes = alb.mul(boxes, image_dims);
            var pre_keep_boxes_arr = boxes.dataSync();
            var scores_arr = scores.dataSync();


            var aa = non_max_suppression(
    pre_keep_boxes_arr,
    scores_arr,
    DEFAULT_IOU_THRESHOLD);
            var keep_indx = aa.Select(p => (float)p.index).ToArray();
            var boxes_arr = aa.Select(p => p.axis).ToList();
            var keep_scores = aa.Select(p => p.score).ToArray();
            var classes_indx_arr = classes.gather(alb.tensor1d(keep_indx)).dataSync();

            var results = new List<YoloBox>();


            for (int i = 0; i < classes_indx_arr.Length; i++)
            {
                var class_indx = (int)classes_indx_arr[i];
                var classProb = keep_scores[i];
                if (classProb < DEFAULT_CLASS_PROB_THRESHOLD)
                {
                    continue;
                }
                var className = class_names[class_indx];
                var top = boxes_arr[i][0];
                var left = boxes_arr[i][1];
                var bottom = boxes_arr[i][2];
                var right = boxes_arr[i][3];
                top = Math.Max(0, top);
                left = Math.Max(0, left);
                bottom = Math.Min(416, bottom);
                right = Math.Min(416, right);
                results.Add(new YoloBox()
                {
                    bottom = (int)bottom,
                    className = className,
                    classProb = classProb,
                    left = (int)left,
                    right = (int)right,
                    top = (int)top
                });
            }
            InvokeProgressEvent(100);
            return results;
        }

    }
    public class Shard
    {
        public string[] paths { get; set; }
        public weight[] weights { get; set; }
    }
    public class weight
    {
        public string name { get; set; }

        public int[] shape { get; set; }
    }
    public class BoundingBox
    {
        public float[] axis;
        public float score;
        public int index;


    }

    public class YoloBox
    {
        public string className { get; set; }
        public float classProb { get; set; }
        public int bottom { get; set; }
        public int top { get; set; }
        public int left { get; set; }
        public int right { get; set; }

    }
}
