import React, { useState } from "react";
import { articleService } from "../services/api";
import type { ArticleCreateDto } from "../types";
import "./AddArticleForm.css";

interface AddArticleFormProps {
  onArticleAdded: () => void;
  onCancel: () => void;
}

const AddArticleForm: React.FC<AddArticleFormProps> = ({
  onArticleAdded,
  onCancel,
}) => {
  const [formData, setFormData] = useState<ArticleCreateDto>({
    title: "",
    category: "",
    publishedAt: new Date().toISOString().split("T")[0],
    articleDetail: {
      summary: "",
      heroImageUrl: "",
      readingTimeSeconds: 0,
    },
  });
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      await articleService.create(formData);
      onArticleAdded();
    } catch (error) {
      console.error("Error creating article:", error);
      alert("Failed to create article");
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (
    e: React.ChangeEvent<
      HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
    >
  ) => {
    const { name, value } = e.target;

    if (name.startsWith("articleDetail.")) {
      const detailField = name.split(".")[1];
      setFormData((prev) => ({
        ...prev,
        articleDetail: {
          ...prev.articleDetail!,
          [detailField]:
            detailField === "readingTimeSeconds" ? parseInt(value) || 0 : value,
        },
      }));
    } else {
      setFormData((prev) => ({
        ...prev,
        [name]: name === "readingTimeSeconds" ? parseInt(value) || 0 : value,
      }));
    }
  };

  return (
    <div className="add-article-modal">
      <div className="modal-content">
        <h2>Add New Article</h2>
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Title:</label>
            <input
              type="text"
              name="title"
              value={formData.title}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label>Category:</label>
            <select
              name="category"
              value={formData.category}
              onChange={handleChange}
              required
            >
              <option value="">Select Category</option>
              <option value="Technology">Technology</option>
              <option value="Business">Business</option>
              <option value="Lifestyle">Lifestyle</option>
            </select>
          </div>

          <div className="form-group">
            <label>Publish Date:</label>
            <input
              type="date"
              name="publishedAt"
              value={formData.publishedAt}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label>Summary:</label>
            <textarea
              name="articleDetail.summary"
              value={formData.articleDetail?.summary || ""}
              onChange={handleChange}
              rows={3}
            />
          </div>

          <div className="form-group">
            <label>Hero Image URL:</label>
            <input
              type="url"
              name="articleDetail.heroImageUrl"
              value={formData.articleDetail?.heroImageUrl || ""}
              onChange={handleChange}
              placeholder="https://example.com/image.jpg"
            />
          </div>

          <div className="form-group">
            <label>Reading Time (seconds):</label>
            <input
              type="number"
              name="articleDetail.readingTimeSeconds"
              value={formData.articleDetail?.readingTimeSeconds || 0}
              onChange={handleChange}
              min="0"
            />
          </div>

          <div className="form-actions">
            <button type="button" onClick={onCancel}>
              Cancel
            </button>
            <button type="submit" disabled={loading}>
              {loading ? "Creating..." : "Create Article"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddArticleForm;
