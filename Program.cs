namespace AncientAi {
    public enum LayerType {
        None,
        Rect,
        Circle,
    }

    public class Program {
        public static void Main(string[] args) {
            var weightsFilepath = "./weights.ppm";
            var imgAmount = 200;
            var imgWidth = 20;
            var imgHeight = 20;

            List<Layer> test_layers = new List<Layer>();
            List<Layer> train_layers = new List<Layer>();
            Ai ai;

            Console.WriteLine("[INFO] Started generating images");
            for (int i = 0; i < imgAmount; ++i) {
                test_layers.Add(Layer.RandomLayer(imgWidth, imgHeight));
                train_layers.Add(Layer.RandomLayer(imgWidth, imgHeight));
            }
            Console.WriteLine("[INFO] Finished generating images");

            ai = new Ai(10f, imgWidth, imgHeight);
            ai.Train(train_layers);
            ai.Weights.WriteTo(weightsFilepath);

            var score = 0;
            foreach (var layer in test_layers) {
                if (ai.Predict(layer) == layer.Type) {
                    score += 1;
                }
            }

            var per = ((float)score / (float)imgAmount) * 100f;
            Console.WriteLine($"The AI prediction score was: {score}/{imgAmount} ({per}%)");
        }
    }

    public class Ai {
        public float Bias;
        public Layer Weights;

        public Ai(float bias, int width, int height) {
            this.Bias = bias;
            this.Weights = new Layer(width, height);
        }

        public Ai(float bias, Layer weights) {
            this.Bias = bias;
            this.Weights = weights;
        }

        public void Train(List<Layer> layers) {
            var count = 1;
            while (true) {
                var score = 0;
                foreach (var layer in layers) {
                    var prediction = Predict(layer);
                    if (prediction != layer.Type) {
                        if (layer.Type == LayerType.Rect) {
                            Weights.RemoveLayer(layer);
                        }
                        else if (layer.Type == LayerType.Circle) {
                            Weights.AddLayer(layer);
                        }
                    }
                    else {
                        score += 1;
                    }
                }

                Console.WriteLine($"[INFO] Train generation: {count} ({score}/{layers.Count})");

                if (score == layers.Count) {
                    break;
                }
                count++;
            }
        }

        public LayerType Predict(Layer layer) {
            var score = 0f;
            for (int y = 0; y < layer.Height; ++y) {
                for (int x = 0; x < layer.Width; ++x) {
                    score += Weights.Values[y, x] * layer.Values[y, x];
                }
            }
            return (score > Bias) ? LayerType.Circle : LayerType.Rect;
        }
    }

    public class Layer {
        public float[,] Values;
        public int Width;
        public int Height;
        public LayerType Type;

        public Layer(int width, int height) {
            this.Width = width;
            this.Height = height;
            this.Values = new float[height, width];
            this.Type = LayerType.None;
        }

        public void AddLayer(Layer layer) {
            for (int y = 0; y < layer.Height; ++y) {
                for (int x = 0; x < layer.Width; ++x) {
                    Values[y, x] += layer.Values[y, x];
                }
            }
        }

        public void RemoveLayer(Layer layer) {
            for (int y = 0; y < layer.Height; ++y) {
                for (int x = 0; x < layer.Width; ++x) {
                    Values[y, x] -= layer.Values[y, x];
                }
            }
        }

        public float GetSmallestNumber() {
            var smallest = Values[0, 0];

            for (int y = 0; y < Height; ++y) {
                for (int x = 0; x < Width; ++x) {
                    if (Values[y, x] < smallest) {
                        smallest = Values[y, x];
                    }
                }
            }

            return smallest;
        }

        public float GetGreatestNumber() {
            var greatest = Values[0, 0];

            for (int y = 0; y < Height; ++y) {
                for (int x = 0; x < Width; ++x) {
                    if (Values[y, x] > greatest) {
                        greatest = Values[y, x];
                    }
                }
            }

            return greatest;
        }

        public void WriteTo(string filepath) {
            if (File.Exists(filepath)) {
                File.Delete(filepath);
            }

            var smallest = GetSmallestNumber();
            var greatest = GetGreatestNumber();

            var normalizedValues = Values;

            // If the smallest number in less than zero we have to shift all numbers
            // so that all numbers are going to be greater than zero
            if (smallest < 0) {
                for (int y = 0; y < Height; ++y) {
                    for (int x = 0; x < Height; ++x) {
                        normalizedValues[y, x] += Math.Abs(smallest);
                    }
                }
                greatest += Math.Abs(smallest);
            }

            File.AppendAllText(filepath, "P3\n");
            File.AppendAllText(filepath, $"# {Type} {smallest}\n");
            File.AppendAllText(filepath, $"{Width} {Height}\n");
            File.AppendAllText(filepath, $"{greatest}\n");

            for (int y = 0; y < Height; ++y) {
                for (int x = 0; x < Width; ++x) {
                    var value = normalizedValues[y, x];
                    File.AppendAllText(filepath, $"{value} {value} {value}\n");
                }
            }
        }

        public static Layer RandomLayer(int width, int height) {
            var random = new Random();
            if (random.Next(2) == 1) {
                return RandomCircle(width, height);
            }
            else {
                return RandomRect(width, height);
            }
        }

        public static Layer RandomCircle(int width, int height) {
            bool inside_circle(int x, int y, int cx, int cy, int cr) {
                return (Math.Pow(x - cx, 2) + Math.Pow(y - cy, 2) <= Math.Pow(cr, 2));
            }

            var layer = new Layer(width, height);
            layer.Type = LayerType.Circle;

            var random = new Random();
            var cx = random.Next(3, width - 3);
            var cy = random.Next(3, height - 3);

            var maxRadius = Math.Min(cx, cy);
            maxRadius = Math.Min(maxRadius, width - 1 - cx);
            maxRadius = Math.Min(maxRadius, height - 1 - cy);

            var radius = random.Next(2, maxRadius);

            for (int y = 0; y < height; ++y) {
                for (int x = 0; x < width; ++x) {
                    if (inside_circle(x, y, cx, cy, radius)) {
                        layer.Values[y, x] = 1f;
                    }
                }
            }

            return layer;
        }

        public static Layer RandomRect(int width, int height) {
            bool inside_rect(int x, int y, int rx, int ry, int rw, int rh) {
                return (x >= rx && x <= rx + rw && y >= ry && y <= ry + rh);
            }

            var layer = new Layer(width, height);
            layer.Type = LayerType.Rect;

            var random = new Random();
            var rx = random.Next(0, width / 2);
            var ry = random.Next(0, height / 2);
            var rw = random.Next(rx + 1, width);
            var rh = random.Next(ry + 1, height);

            for (int y = 0; y < height; ++y) {
                for (int x = 0; x < width; ++x) {
                    if (inside_rect(x, y, rx, ry, rw, rh)) {
                        layer.Values[y, x] = 1f;
                    }
                }
            }

            return layer;
        }
    }
}
