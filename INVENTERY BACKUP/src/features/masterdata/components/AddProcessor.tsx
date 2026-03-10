import { useState, useEffect } from "react";

import {
  addProcessor,
  getProcessors,
  updateProcessor,
} from "../services/processorService";

import GenericAddMasterData from "./GenericAddMasterData";

import type { ProcessorItem } from "@features/masterdata/types";

function AddProcessor() {
  const [processorName, setProcessorName] = useState("");
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [processorList, setProcessorList] = useState<ProcessorItem[]>([]);

  // Fetch processor list on component mount
  useEffect(() => {
    const fetchProcessors = async () => {
      const data = await getProcessors();
      setProcessorList(data);
    };
    fetchProcessors();
  }, []);

  const handleSubmit = async () => {
    if (!processorName) {
      setSubmitError("Please enter a processor name.");
      return;
    }
    setIsSubmitting(true);
    setSubmitError(null);
    const ProcessorData: ProcessorItem = {
      Name: processorName,
    };
    const result = await addProcessor(ProcessorData);
    setIsSubmitting(false);
    if (typeof result === "string") {
      setSubmitError(result);
    } else {
      setProcessorName("");
      // Refresh the list after adding a new processor
      const data = await getProcessors();
      setProcessorList(data);
    }
  };

  return (
    <GenericAddMasterData
      title="Add Processor"
      label="Processor Name"
      inputValue={processorName}
      onInputChange={setProcessorName}
      onSubmit={handleSubmit}
      isSubmitting={isSubmitting}
      error={submitError}
      items={processorList}
      onUpdate={async (updatedItem) => {
        const result = await updateProcessor(updatedItem);
        if (result.success) {
          // Refresh the list after updating
          const data = await getProcessors();
          setProcessorList(data);
        }
        return result;
      }}
    />
  );
}

export default AddProcessor;
