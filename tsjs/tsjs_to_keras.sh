#!/usr/bin/env bash
# in win10, an 0x0d might be inserted at the end of line and cause bash problem
tensorflowjs_converter     --input_format tfjs_layers_model --output_format keras ./model_as_tsjs/model.json ./model_as_keras/model.h5
