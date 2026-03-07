"""
Dead Cells - Archipelago World
base_classes.py

Base item and location classes for Dead Cells.
"""

from BaseClasses import Item, ItemClassification


class DeadCellsItem(Item):
    """A single item in the Dead Cells randomizer."""
    game: str = "Dead Cells"
