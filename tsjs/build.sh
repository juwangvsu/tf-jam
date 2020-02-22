#!/usr/bin/env bash
node index.js
bash tsjs_to_keras.sh
#tensorflowjs_converter     --input_format tfjs_layers_model --output_format keras ./model_as_tsjs/model.json ./model_as_keras/model.h5

python3 keras_to_pb.py
cp frozen2.pb.bytes ../Assets/Resources/frozen2.pb.bytes
python3 pb_to_tensorboard.py

#python3 /usr/local/lib/python3.5/dist-packages/tensorflow/python/tools/import_pb_to_tensorboard.py  --model_dir ./frozen2.pb.bytes --log_dir log3
#tensorboard --logdir log3
#bash pb_to_frozen_pb.sh

