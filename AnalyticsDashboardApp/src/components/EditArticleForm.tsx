import React, { useState } from "react";
import { articleService } from "../services/api";
import type { ArticleDto, ArticleUpdateDto } from "../types";
import "./AddArticleForm.css";

interface EditArticleFormProps {
  article: ArticleDto;
  onArticleUpdated: () => void;
  onCancel: () => void;
}

const EditArticleForm: React.FC<EditArticleFormProps> = ({
  article,
  onArticleUpdated,
  onCancel,
}) => {
  const [formData, setFormData] = useState<ArticleUpdateDto>({
    title: article.title,
    category: article.category,
    publishedAt: new Date(article.publishedAt).toISOString().split("T")[0],
    articleDetail: article.articleDetail
      ? {
          summary: article.articleDetail.summary,
          heroImageUrl: article.articleDetail.heroImageUrl,
          readingTimeSeconds: article.articleDetail.readingTimeSeconds,
        }
      : {
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
      await articleService.update(article.id, formData);
      onArticleUpdated();
    } catch (error) {
      console.error("Error updating article:", error);
      alert("Failed to update article");
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
        [name]: value,
      }));
    }
  };

  return (
    <div className="add-article-modal">
      <div className="modal-content">
        <h2>Edit Article</h2>
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
            <label>Summary:</label>
            <textarea
              name="articleDetail.summary"
              value={formData.articleDetail?.summary || ""}
              onChange={handleChange}
              rows={3}
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
              {loading ? "Updating..." : "Update Article"}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default EditArticleForm;
