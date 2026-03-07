import { useState, useEffect } from "react";

import { addModel, getModels, updateModel } from "../services/modelService";

import GenericAddMasterData from "./GenericAddMasterData";

import type { ModelItem } from "@features/masterdata/types";

function AddModel() {
  const [modelName, setModelName] = useState("");
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [models, setModels] = useState<ModelItem[]>([]);

  // Fetch models on component mount
  useEffect(() => {
    const fetchModels = async () => {
      const data = await getModels();
      setModels(data);
    };
    fetchModels();
  }, []);

  const handleSubmit = async () => {
    if (!modelName) {
      setSubmitError("Please enter a model name.");
      return;
    }
    setIsSubmitting(true);
    setSubmitError(null);
    const modelData = {
      Name: modelName,
    };
    const result = await addModel(modelData);
    setIsSubmitting(false);
    if (typeof result === "string") {
      setSubmitError(result);
    } else {
      setModelName("");
      // Refresh the list after adding a new model
      const data = await getModels();
      setModels(data);
    }
  };

  return (
    <GenericAddMasterData
      title="Add Model"
      label="Model Name"
      inputValue={modelName}
      onInputChange={setModelName}
      onSubmit={handleSubmit}
      isSubmitting={isSubmitting}
      error={submitError}
      items={models}
      onUpdate={async (updatedItem) => {
        const result = await updateModel(updatedItem);
        if (result.success) {
          // Refresh the list after updating
          const data = await getModels();
          setModels(data);
        }
        return result;
      }}
    />
  );
}

export default AddModel;
