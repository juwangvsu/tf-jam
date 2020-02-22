#2/210/2020 this not working so far, it is done in keras_to_pb.py
./freeze_graph --input_graph=./model_as_pb/model.pb --input_checkpoint=./model_as_checkpoint/Profile.ckpt --output_node_name=shots/BiasAdd --output_graph=../Assets/Resources/frozen.pb.bytes
